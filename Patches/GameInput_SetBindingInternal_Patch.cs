using HarmonyLib;
using System;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{

    /*
     * Called when QuickSlot 1-5 keybinding is set.
     */
    [HarmonyPatch(typeof(GameInput), "SafeSetBinding")]
    class GameInput_SafeSetBinding_Patch
    {
        static void Postfix()
        {
            Mod.RedrawQuickSlots();
        }
    }

    [HarmonyPatch(typeof(GameInput), "Awake")]
    class GameInput_Awake_Patch
    {
        static void Postfix()
        {
            GameInput.OnPrimaryDeviceChanged += Mod.RedrawQuickSlots;
        }
    }
}
