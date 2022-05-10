using HarmonyLib;
using QModManager.Utility;
using QuickSlotsPlus.Utility;
using System.Collections.Generic;
using System.Reflection.Emit;


/*
 * Fixes a bug from MoreQuickSlots
 */
namespace QuickSlotsPlus.Patches
{

    /***
     * When the QuickSlots.slotNames variable is overwritten, it sometimes resets back to the hardcoded length of 5. When this
     * happens it causes index out of range exceptions when you select the added quick slots in game and eventually causes strange
     * flickering and makes items unusable. I couldn't determine what triggers this, but we can transpile the few methods that
     * call the "slotNames" variable and load our own to avoid it.
     */
    [HarmonyPatch(typeof(QuickSlots))]
    [HarmonyPatch("Update")]
    class QuickSlots_SlotNames_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld)
                {
                    codes[i].operand = AccessTools.Field(typeof(LabelUtil), "slotNames");
                    Logger.Log(Logger.Level.Info, "Transpiled Update method with new slotnames.");
                    break;
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(QuickSlots))]
    [HarmonyPatch("SelectInternal")]
    class QuickSlots_SelectInternal_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i + 1].opcode == OpCodes.Ldarg_1)
                {
                    codes[i].operand = AccessTools.Field(typeof(LabelUtil), "slotNames");
                    Logger.Log(Logger.Level.Info, "Transpiled SelectInternal method with new slotnames.");
                    break;
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(QuickSlots))]
    [HarmonyPatch("DeselectInternal")]
    class QuickSlots_DeselectInternal_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i + 1].opcode == OpCodes.Ldloc_0)
                {
                    codes[i].operand = AccessTools.Field(typeof(LabelUtil), "slotNames");
                    Logger.Log(Logger.Level.Info, "Transpiled DeselectInternal method with new slotnames.");
                    break;
                }
            }
            return codes;
        }
    }
}
