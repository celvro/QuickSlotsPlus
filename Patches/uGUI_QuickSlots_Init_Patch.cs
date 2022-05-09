using HarmonyLib;
using QuickSlotsPlus.Utility;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{
    [HarmonyPatch(typeof(uGUI_QuickSlots), "Init")]
    public static class uGUI_QuickSlots_Init_Patch
    {
        public static void Postfix(uGUI_QuickSlots __instance)
        {
            DrawLabels(__instance);

            Logger.Log(Logger.Level.Info, "Patched labels.");
        }

        /**
         * TODO: none of this works without TextMeshPro, maybe it can be replaced with UnityEngine.TextRenderingModule?
         */
        public static void DrawLabels(uGUI_QuickSlots quickSlots)
        {
            // Do nothing until this actually works, need to comment out failing code
            return;
            /*if (!ShouldShowLabels(quickSlots)) {
                return;
            }

            // Reading in "customLabels.json", keep it outside the loop
            LabelUtil.LoadCustomLabels();

            for (var i = 0; i < Mod.Config.slotCount; i++)
            {
                var icon = quickSlots.GetIcon(i);

                CreateNewText(GetTextPrefab(), icon.transform, LabelUtil.getSlotKeyText(i), i);
            }*/
        }

        /*private static bool ShouldShowLabels(uGUI_QuickSlots quickSlots)
        {
            if (quickSlots == null || !Mod.Config.showLabels)
            {
                return false;
            }
            Player player = Player.main;
            return player != null && player.GetMode() != Player.Mode.Piloting;
        }*/

        /*private static TextMeshProUGUI GetTextPrefab()
        {
            var prefabObject = Object.FindObjectOfType<HandReticle>();

            return prefabObject?.compTextHandSubscript;
        }

        // https://github.com/DanielLavrushin/SaubNauticaBZ_QuickSlotsMod/blob/fc02a3c73d76aad3aa03aa08eb1a18bb467d00ac/QuickSlots/GameController.cs#L122
        private static TextMeshProUGUI CreateNewText(TextMeshProUGUI prefab, Transform parent, string newText, int index = -1)
        {
            TextMeshProUGUI text = Object.Instantiate(prefab);
            text.gameObject.layer = parent.gameObject.layer;
            text.gameObject.name = "QuickSlotText" + index.ToString();
            text.transform.SetParent(parent, false);
            text.transform.localScale = new Vector3(1, 1, 1);
            text.gameObject.SetActive(true);
            text.enabled = true;
            text.text = newText;
            text.fontSize = Mod.Config.labelSize;
            RectTransformExtensions.SetParams(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            text.rectTransform.anchoredPosition = new Vector3(0, -36);
            text.alignment = TextAlignmentOptions.Midline;
            text.raycastTarget = false;

            return text;
        }*/
    }
}
