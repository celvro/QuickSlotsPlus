using HarmonyLib;
using QuickSlotsPlus.Utility;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{
    /*
     * Draws hotkey labels. 
     */
    [HarmonyPatch(typeof(uGUI_QuickSlots), "Init")]
    public static class uGUI_QuickSlots_Init_Patch
    {
        private static FieldInfo uGUI_QuickSlots_icons = typeof(uGUI_QuickSlots).GetField("icons", BindingFlags.Instance | BindingFlags.NonPublic);
        public static void Postfix(uGUI_QuickSlots __instance)
        {
            DrawLabels(__instance);

            Logger.Log(Logger.Level.Info, "Patched hotkey labels.");
        }
 
        public static void DrawLabels(uGUI_QuickSlots quickSlots)
        {
            uGUI_ItemIcon[] icons = (uGUI_ItemIcon[])uGUI_QuickSlots_icons.GetValue(quickSlots);
            if (!ShouldShowLabels(quickSlots) || icons == null)
            {
                return;
            }

            // Reading in "customLabels.json", keep it outside the loop
            LabelUtil.LoadCustomLabels();

            Text textPrefab = GetTextPrefab();
            if (textPrefab == null)
            {
                Logger.Log(Logger.Level.Warn, "Could not find text prefab.");
                return;
            }

            for (var i = 0; i < Mod.Config.slotCount; i++)
            {
                uGUI_ItemIcon itemIcon = icons[i];
                CreateRaycastTarget(icons[i]);
                CreateNewText(textPrefab, itemIcon.transform, LabelUtil.getSlotKeyText(i), i);
            }
        }

        /**
         * Create a bigger raycast target for QuickSnap to work more consistently. Maybe
         * the quick slot icon raycast centers are off or something idk.
         */
        private static void CreateRaycastTarget(uGUI_ItemIcon icon)
        {
            return; // TODO
        }

        private static Text GetTextPrefab()
        {
            HandReticle handReticle = Object.FindObjectOfType<HandReticle>();
            if (handReticle != null)
            {
                Text interactPrimaryText = handReticle.interactPrimaryText;
                if (interactPrimaryText != null)
                {
                    return interactPrimaryText;
                }
            }

            return null;
        }
        private static bool ShouldShowLabels(uGUI_QuickSlots quickSlots)
        {
            if (quickSlots == null || !Mod.Config.showLabels)
            {
                return false;
            }
            Player player = Player.main;

            // I don't think the piloting check works...
            return player != null && player.GetMode() != Player.Mode.Piloting;
        }

        // Used dnspy to rip the bones from RandyKnapp's version: https://www.nexusmods.com/subnautica/mods/14
        private static Text CreateNewText(Text prefab, Transform parent, string newText, int index = -1)
        {
            Text text = Object.Instantiate(prefab);
            text.gameObject.layer = parent.gameObject.layer;
            text.gameObject.name = "QuickSlotText" + ((index >= 0) ? index.ToString() : "");
            text.transform.SetParent(parent, false);
            text.transform.localScale = new Vector3(1f, 1f, 1f);
            text.gameObject.SetActive(true);
            text.enabled = true;
            text.text = newText;
            text.alignment = TextAnchor.MiddleCenter;
            RectTransformExtensions.SetParams(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            text.rectTransform.SetSizeWithCurrentAnchors(0, 100f);
            text.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, 100f);
            text.raycastTarget = false;

            // **** Set config options ****
            text.fontSize = Mod.Config.labelSize;

            float xPos = Mod.Config.labelXpos;
            float yPos = Mod.Config.labelYpos - 36f; // Subtract 36 to make default position be below the icons
            text.rectTransform.anchoredPosition = new Vector3(xPos, yPos);

            return text;
        }
    }
}
