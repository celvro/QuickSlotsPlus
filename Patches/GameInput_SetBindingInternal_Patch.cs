using HarmonyLib;
using System;

namespace QuickSlotsPlus.Patches
{

    /*
     * Called when QuickSlot 1-5 keybinding is set.
     */
    [HarmonyPatch(typeof(GameInput), "SetBindingInternal", new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(int) })]
    class GameInput_SetBindingInternal_Patch
    {
        static void Postfix()
        {
            Mod.RedrawQuickSlots();
        }
    }
}
