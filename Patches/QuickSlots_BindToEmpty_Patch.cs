using HarmonyLib;

namespace QuickSlotsPlus.Patches
{

    /***
     * Prevent adding new items to the QuickSlot bar.
     */
    [HarmonyPatch(typeof(QuickSlots), "BindToEmpty")]
    class QuickSlots_BindToEmpty_Patch
    {
        static bool Prefix()
        {
            return !Mod.Config.disableBindToEmpty;
        }
    }
}
