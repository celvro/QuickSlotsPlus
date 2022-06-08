using HarmonyLib;
using QuickSlotsPlus.Utility;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{
    /*
     * Draws hotkey labels. 
     */
    [HarmonyPatch(typeof(uGUI_QuickSlots), "Init")]
    public static class uGUI_QuickSlots_Init_Patch
    {
        public static void Postfix(uGUI_QuickSlots __instance)
        {
            LabelUtil.DrawLabels(__instance);

            Logger.Log(Logger.Level.Info, "Patched hotkey labels.");
        }
    }
}
