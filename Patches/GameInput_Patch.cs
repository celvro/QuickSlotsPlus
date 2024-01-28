using HarmonyLib;
using System;

namespace QuickSlotsPlus.Patches
{

    /*
     * Called when QuickSlot 1-5 keybinding is set.
     */
    [HarmonyPatch(typeof(GameInput), "SafeSetBinding", new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(string) })]
    class GameInput_SafeSetBinding_Patch
    {
        static void Postfix()
        {
            Mod.logger.LogDebug("Redraw quickslots after SafeSetBinding");
            Mod.RedrawQuickSlots();
        }
    }

    [HarmonyPatch(typeof(GameInput), "Awake")]
    class GameInput_Awake_Patch
    {
        static void Postfix()
        {
            GameInput.OnPrimaryDeviceChanged += Redraw;
        }

        static void Redraw()
        {
            if (!Mod.Options.mixedInputMode)
            {
                Mod.logger.LogDebug("Redraw quickslots after Input change");
                Mod.RedrawQuickSlots();
            }
        }
    }
}
