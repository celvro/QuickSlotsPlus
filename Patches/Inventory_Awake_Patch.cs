using HarmonyLib;

namespace QuickSlotsPlus.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
    public class Inventory_Awake_Patch
    {
        // https://github.com/DanielLavrushin/SaubNauticaBZ_QuickSlotsMod/blob/fc02a3c73d76aad3aa03aa08eb1a18bb467d00ac/QuickSlots/Patches/Inventory_Awake_Patch.cs#L16
        public static void Postfix(Inventory __instance)
        {
            int slotCount = QuickSlotsPlus.Options.slotCount;
            Player player = __instance.GetComponent<Player>();

            __instance.quickSlots = new QuickSlots(__instance.gameObject, __instance.toolSocket, __instance.cameraSocket, __instance, player.rightHandSlot, slotCount);
        }
    }
}
