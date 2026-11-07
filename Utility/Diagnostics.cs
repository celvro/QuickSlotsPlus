#if SUBNAUTICA
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace QuickSlotsPlus.Utility
{
    /***
     * TEMPORARY diagnostic logging, for the "slots 6-20 hotkeys do nothing while scrolling still works" report.
     * Delete this file and its calls in QuickSlotsPlus.Update before cutting the next release.
     *
     * Slots 1-5 are read by the vanilla uGUI_QuickSlots.HandleInput; slots 6-20 are read only by our own
     * Update(). Nothing on that path logs, so a user's BepInEx log can't currently distinguish between:
     *   1. Update() never running,
     *   2. GameInput.GetButtonDown returning false (the action is missing from GameInputSystem.actions,
     *      or is disabled, or has no binding),
     *   3. SlotKeyDown running but Select() no-opping because quickSlots.slotCount is still 5,
     *   4. SlotKeyDown being swallowed by its PDA / AvatarInputHandler / Player.Mode gates.
     * These logs tell the four apart.
     */
    public static class Diagnostics
    {
        private const float HeartbeatSeconds = 10f;

        private static readonly (string name, GameInput.Button button)[] vanillaSlotButtons =
        {
            ("Slot1", GameInput.Button.Slot1),
            ("Slot2", GameInput.Button.Slot2),
            ("Slot3", GameInput.Button.Slot3),
            ("Slot4", GameInput.Button.Slot4),
            ("Slot5", GameInput.Button.Slot5)
        };

        private static float nextHeartbeat;
        private static bool loggedActions;

        /*
         * Proves Update() is actually ticking, and that the enlarged QuickSlots object is the installed one.
         */
        public static void Tick()
        {
            if (Time.unscaledTime < nextHeartbeat)
            {
                return;
            }
            nextHeartbeat = Time.unscaledTime + HeartbeatSeconds;

            QuickSlots quickSlots = Inventory.main == null ? null : Inventory.main.quickSlots;

            QuickSlotsPlus.logger.LogInfo(
                "[diag] tick:" +
                $" GameInput.IsInitialized={GameInput.IsInitialized}" +
                $", Options.slotCount={QuickSlotsPlus.Options.slotCount}" +
                $", quickSlots.GetSlotCount()={Describe(quickSlots, qs => qs.GetSlotCount())}" +
                $", binding.Length={Describe(quickSlots, qs => qs.binding == null ? -1 : qs.binding.Length)}" +
                $", activeSlot={Describe(quickSlots, qs => qs.GetActiveSlotID())}" +
                $", slotActionsRegistered={CountRegisteredSlotActions()}");

            LogRegisteredActionsOnce();
        }

        /*
         * Vanilla handles slots 1-5 itself; we only log them so the user's log shows whether a 1-5 press is
         * seen in the same session where a 6-20 press is not. If "Slot3 down" appears and "Slot8 down" never
         * does, the break is in GameInput, not in our Update() or in QuickSlots.
         */
        public static void LogVanillaSlotPresses()
        {
            foreach ((string name, GameInput.Button button) in vanillaSlotButtons)
            {
                if (GameInput.GetButtonDown(button))
                {
                    QuickSlotsPlus.logger.LogInfo($"[diag] vanilla {name} down (handled by uGUI_QuickSlots, not by us)");
                }
            }
        }

        public static void LogSlotKeyDown(GameInput.Button button, int slotIndex, QuickSlots quickSlots)
        {
            string binding = GameInput.GetBinding(GameInput.PrimaryDevice, button, GameInput.BindingSet.Primary);

            QuickSlotsPlus.logger.LogInfo(
                $"[diag] {button} down -> SlotKeyDown({slotIndex}):" +
                $" binding={binding}" +
                $", slotCount={Describe(quickSlots, qs => qs.GetSlotCount())}" +
                $", activeSlot={Describe(quickSlots, qs => qs.GetActiveSlotID())}" +
                $", pdaInUse={DescribePdaInUse()}" +
                $", avatarInputEnabled={DescribeAvatarInputEnabled()}" +
                $", playerMode={DescribePlayerMode()}");
        }

        /*
         * SlotKeyDown -> Select() silently no-ops when slotIndex >= slotCount, so the only way to see whether
         * it took effect is the selection afterwards.
         */
        public static void LogSlotKeyDownResult(int slotIndex, QuickSlots quickSlots)
        {
            int active = quickSlots == null ? int.MinValue : quickSlots.GetActiveSlotID();
            string verdict = active == slotIndex ? "selected" : (active == -1 ? "deselected (toggled off)" : "NOT selected");

            QuickSlotsPlus.logger.LogInfo($"[diag]   after SlotKeyDown({slotIndex}): activeSlot={active} -> {verdict}");
        }

        /*
         * The decisive check. GameInputSystem.GetButtonState does actions.TryGetValue(action, ...) and returns
         * None when the action is absent, so a button that was registered and bound can still be completely
         * dead if it never made it into that dictionary.
         */
        private static void LogRegisteredActionsOnce()
        {
            if (loggedActions || !GameInput.IsInitialized)
            {
                return;
            }
            loggedActions = true;

            if (!TryGetActions(out IDictionary actions))
            {
                QuickSlotsPlus.logger.LogWarning("[diag] could not read GameInputSystem.actions");
                return;
            }

            QuickSlotsPlus.logger.LogInfo($"[diag] GameInputSystem.actions holds {actions.Count} actions");

            foreach ((string name, GameInput.Button button) in vanillaSlotButtons.Concat(ExtraSlotButtons()))
            {
                if (!actions.Contains(button))
                {
                    QuickSlotsPlus.logger.LogWarning($"[diag]   {name} ({(int)button}): MISSING from GameInputSystem.actions - this key can never fire");
                    continue;
                }

                object action = actions[button];
                QuickSlotsPlus.logger.LogInfo(
                    $"[diag]   {name} ({(int)button}): present" +
                    $", enabled={ReadProperty(action, "enabled")}" +
                    $", bindings={ReadBindingCount(action)}" +
                    $", savedBinding={GameInput.GetBinding(GameInput.PrimaryDevice, button, GameInput.BindingSet.Primary)}");
            }
        }

        /*
         * Cheap re-check on every heartbeat: GameInputSystem.Deinitialize/Initialize can cycle (e.g. on a
         * device change) and rebuild the dictionary, so "present at startup" isn't "present now".
         */
        private static string CountRegisteredSlotActions()
        {
            if (!TryGetActions(out IDictionary actions))
            {
                return "?";
            }

            var all = vanillaSlotButtons.Concat(ExtraSlotButtons()).ToArray();
            int present = all.Count(slot => actions.Contains(slot.button));

            return $"{present}/{all.Length}";
        }

        private static bool TryGetActions(out IDictionary actions)
        {
            actions = null;

            if (!GameInput.IsInitialized)
            {
                return false;
            }

            object input = AccessTools.Field(typeof(GameInput), "input")?.GetValue(null);
            if (input == null)
            {
                return false;
            }

            actions = AccessTools.Field(input.GetType(), "actions")?.GetValue(input) as IDictionary;

            return actions != null;
        }

        private static IEnumerable<(string name, GameInput.Button button)> ExtraSlotButtons()
        {
            for (int i = 0; i < QuickSlotsPlus.ExtraSlotButtons.Length; i++)
            {
                yield return ("Slot" + (i + 6), QuickSlotsPlus.ExtraSlotButtons[i]);
            }
        }

        private static string ReadProperty(object target, string name)
        {
            PropertyInfo property = target?.GetType().GetProperty(name);
            return property == null ? "?" : property.GetValue(target, null)?.ToString() ?? "null";
        }

        private static string ReadBindingCount(object action)
        {
            object bindings = action?.GetType().GetProperty("bindings")?.GetValue(action, null);
            return ReadProperty(bindings, "Count");
        }

        private static string Describe<T>(QuickSlots quickSlots, System.Func<QuickSlots, T> read)
        {
            return quickSlots == null ? "n/a" : read(quickSlots).ToString();
        }

        private static string DescribePdaInUse()
        {
            if (Player.main == null)
            {
                return "n/a";
            }
            PDA pda = Player.main.GetPDA();
            return pda == null ? "n/a" : pda.isInUse.ToString();
        }

        private static string DescribeAvatarInputEnabled()
        {
            return AvatarInputHandler.main == null ? "n/a" : AvatarInputHandler.main.IsEnabled().ToString();
        }

        private static string DescribePlayerMode()
        {
            return Player.main == null ? "n/a" : Player.main.GetMode().ToString();
        }
    }
}
#endif
