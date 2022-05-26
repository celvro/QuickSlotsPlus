using HarmonyLib;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{

    /***
     * Prevent adding new items to the QuickSlot bar.
     */
    [HarmonyPatch(typeof(QuickSlots), "OnAddItem")]
    class QuickSlots_OnAddItem_Patch
    {
        static bool Prefix(QuickSlots __instance, InventoryItem item)
        {
            return !Mod.Config.disableBindToEmpty || IntroVignette.isIntroActive;
        }
    }
}
