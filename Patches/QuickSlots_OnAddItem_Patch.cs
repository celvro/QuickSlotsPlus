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
            Logger.Log(Logger.Level.Debug, $"Tech type is {item.item.GetTechType()}");
            if (item.item.GetTechType() == TechType.FireExtinguisher)
            {
                Logger.Log(Logger.Level.Debug, $"Picked up fire extinguisher!");
                // Add exception if bindings are empty so you can use the FireExtinguisher at the beginning of the game
                // It won't let you use the PDA until after you put out the fire
                for (int i = 0; i < __instance.binding.Length; i++)
                {
                    if (__instance.binding[i] != null)
                    {
                        Logger.Log(Logger.Level.Debug, "Bindings are not empty!");
                        return !Mod.Config.disableBindToEmpty;
                    }
                }
                Logger.Log(Logger.Level.Debug, "Binding fire extinguisher so you can get through the intro.");
                return true;
            }

            return !Mod.Config.disableBindToEmpty;
        }
    }
}
