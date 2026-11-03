#if SUBNAUTICA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace QuickSlotsPlus.Patches
{

    /***
     * Stops the game hanging on the epilepsy warning screen when Nitrox runs with Nautilus 1.0.0-pre.45 or newer, which this
     * mod requires.
     *
     * Nitrox adds its keybinds as GameInput.Button values from 1000 up by patching Enum.GetValues, then looks up their actions
     * during GameInputSystem.Initialize. The game creates those actions from GameInput.AllActions, a copy of Enum.GetValues that
     * is only taken once. When that copy doesn't include Nitrox's buttons the lookup throws, Initialize never finishes, and the
     * game has no input. So add any that are missing to the list Initialize reads.
     *
     * AllActions itself is left alone, so Nitrox's keybinds keep their defaults but can't be rebound in the options menu.
     * https://github.com/SubnauticaModding/Nautilus/issues/689
     */
    [HarmonyPatch(typeof(GameInputSystem), nameof(GameInputSystem.Initialize))]
    public class GameInputSystem_Initialize_Patch
    {
        private const int NitroxBaseId = 1000;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var allActions = AccessTools.Field(typeof(GameInput), nameof(GameInput.AllActions));
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].LoadsField(allActions))
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameInputSystem_Initialize_Patch), nameof(AddMissingNitroxButtons))));
                    QuickSlotsPlus.logger.LogInfo("Transpiled GameInputSystem.Initialize for Nitrox keybinds.");
                    return codes;
                }
            }

            QuickSlotsPlus.logger.LogWarning("Could not find GameInput.AllActions in GameInputSystem.Initialize, Nitrox may hang on startup.");
            return codes;
        }

        public static GameInput.Button[] AddMissingNitroxButtons(GameInput.Button[] allActions)
        {
            GameInput.Button[] missing = Enum.GetValues(typeof(GameInput.Button)).Cast<GameInput.Button>()
                .Where(button => (int)button >= NitroxBaseId && !allActions.Contains(button))
                .ToArray();

            if (missing.Length == 0)
            {
                return allActions;
            }

            QuickSlotsPlus.logger.LogInfo($"Added missing Nitrox buttons to GameInputSystem.Initialize: {string.Join(", ", missing.Select(button => (int)button))}");
            return allActions.Concat(missing).ToArray();
        }
    }
}
#endif
