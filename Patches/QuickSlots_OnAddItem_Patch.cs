using HarmonyLib;

namespace QuickSlotsPlus.Patches
{

    /***
     * Prevent adding new items to the QuickSlot bar.
     */
    [HarmonyPatch(typeof(QuickSlots), "BindToEmpty")]
    class QuickSlots_OnAddItem_Patch
    {
        static bool Prefix(ref int __result)
        {
            bool shouldDisable = Mod.Config.disableBindToEmpty;
            if (shouldDisable)
            {
                __result = -1;
                return false;
            }
            return true;
        }
    }
}
