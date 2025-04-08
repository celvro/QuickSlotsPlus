using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSlotsPlus
{
    [Menu("QuickSlots+")]
    public class StandardConfig : ConfigFile
    {
        [Slider("QuickSlot Count", 0, 20, DefaultValue = 10, Tooltip = "Reload the game if you reduce this below 5."), OnChange(nameof(RedrawQuickSlots))]
        public int slotCount = 10;

        [Toggle("Do Not Add New Items to Empty Slots", Tooltip = "You can override this for individual slots in BepInEx/plugins/QuickSlotsPlus/allowItems.json")]
        public bool disableBindToEmpty = true;

        [Toggle("Mixed Input Mode", Tooltip = "Prevent items from unequiping, and labels from switching, when using keyboard and controller at the same time."), OnChange(nameof(RedrawQuickSlots))]
        public bool mixedInputMode = false;

        [Toggle("Show HotKey labels"), OnChange(nameof(RedrawQuickSlots))]
        public bool showLabels = true;

        [Toggle("Show labels while piloting Vehicles", Tooltip = "Exit vehicle to take effect")]
        public bool showLabelsWhilePiloting = false;

        [ColorPicker("Label Color", Advanced = true), OnChange(nameof(RedrawQuickSlots))]
        public Color color = Color.white;

        [Slider("Label Size", 8, 100, DefaultValue = 20), OnChange(nameof(RedrawQuickSlots))]
        public float labelSize = 20;

        [Slider("Horizontal Position", -20, 20, DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelXpos = 0;

        [Slider("Vertical Position", -25, 100, DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelYpos = 0;

        [Keybind("Slot 6"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey6 = KeyCode.Alpha6;

        [Keybind("Slot 7"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey7 = KeyCode.Alpha7;

        [Keybind("Slot 8"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey8 = KeyCode.Alpha8;

        [Keybind("Slot 9"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey9 = KeyCode.Alpha9;

        [Keybind("Slot 10"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey10 = KeyCode.Alpha0;

        [Keybind("Slot 11"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey11 = KeyCode.Minus;

        [Keybind("Slot 12"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey12 = KeyCode.Equals;

        [Keybind("Slot 13"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey13;

        [Keybind("Slot 14"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey14;

        [Keybind("Slot 15"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey15;

        [Keybind("Slot 16"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey16;

        [Keybind("Slot 17"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey17;

        [Keybind("Slot 18"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey18;

        [Keybind("Slot 19"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey19;

        [Keybind("Slot 20"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey20;

        /*[Button("Reset Keybinds", Tooltip = "Reset Keybinds to their default values. Close options menu to take effect.")]*/
        public void ResetKeybinds(ButtonClickedEventArgs eventArgs)
        {
            Mod.Options.HotKey6 = KeyCode.Alpha6;
            Mod.Options.HotKey7 = KeyCode.Alpha7;
            Mod.Options.HotKey8 = KeyCode.Alpha8;
            Mod.Options.HotKey9 = KeyCode.Alpha9;
            Mod.Options.HotKey10 = KeyCode.Alpha0;
            Mod.Options.HotKey11 = KeyCode.Minus;
            Mod.Options.HotKey12 = KeyCode.Equals;
            Mod.Options.HotKey13 = KeyCode.None;
            Mod.Options.HotKey14 = KeyCode.None;
            Mod.Options.HotKey15 = KeyCode.None;
            Mod.Options.HotKey16 = KeyCode.None;
            Mod.Options.HotKey17 = KeyCode.None;
            Mod.Options.HotKey18 = KeyCode.None;
            Mod.Options.HotKey19 = KeyCode.None;
            Mod.Options.HotKey20 = KeyCode.None;

            Mod.Options.Save();
            Mod.logger.LogDebug("Clicked reset keybinds.");
        }

        /* 
         * Redraw the QuickSlots after user changes preferences in settings menu.
         */
        public static void RedrawQuickSlots()
        {
            Mod.logger.LogDebug("Redraw quick slots due to options change");
            Mod.RedrawQuickSlots();
        }
    }
}
