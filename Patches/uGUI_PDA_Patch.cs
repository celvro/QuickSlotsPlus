using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;


namespace QuickSlotsPlus.Patches
{

    [HarmonyPatch(typeof(uGUI_PDA), "OnOpenPDA")]
    class uGUI_PDA_Patch
    {
        static void Postfix(uGUI_PDA __instance, PDATab __0)
        {
            uGUI_Block blocker = __instance.GetComponentInChildren<uGUI_Block>();
            MoveTransformUp(blocker.transform);
            
            uGUI_ItemDropTarget[] targets = __instance.GetComponentsInChildren<uGUI_ItemDropTarget>();
            foreach(uGUI_ItemDropTarget target in targets)
            {
                MoveTransformUp(target.transform);
            }
            Logger.Log(Logger.Level.Info, "Moved uGUI_Block and uGUI_ItemDropTarget(s) up so they don't cover quick slots.");
        }

        /*
         * Move uGUI_Block items up so they're not on top of the quick slots when you open the PDA =/
         * 
         * This should fix quickslot snapping.
         */
        private static void MoveTransformUp(Transform transform)
        {
            var temp = transform.localPosition;
            temp.y = 75f;
            transform.localPosition = temp;
        }
    }

}
