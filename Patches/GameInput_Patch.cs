using System;
using HarmonyLib;

namespace QuickSlotsPlus.Patches
{

    /*
     * Called when any keybindings are set.
     */
    [HarmonyPatch(typeof(GameInput), nameof(GameInput.TryBind), new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(string) })]
    class GameInput_Patch
    {
        static void Postfix()
        {
            QuickSlotsPlus.logger.LogDebug("Redraw quickslots after TryBind");
            QuickSlotsPlus.RedrawQuickSlots();
        }
    }
}
