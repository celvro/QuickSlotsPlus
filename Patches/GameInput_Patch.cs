using System;
using HarmonyLib;

namespace QuickSlotsPlus.Patches
{

    /*
     * Called when any keybindings are set.
     */
#if SUBNAUTICA
    [HarmonyPatch(typeof(GameInput), nameof(GameInput.TryBind), new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(string) })]
#else
    [HarmonyPatch(typeof(GameInput), nameof(GameInput.SafeSetBinding), new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(string) })]
#endif
    class GameInput_Patch
    {
        static void Postfix()
        {
            QuickSlotsPlus.logger.LogDebug("Redraw quickslots after keybind change");
            QuickSlotsPlus.RedrawQuickSlots();
        }
    }
}
