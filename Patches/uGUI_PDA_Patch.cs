using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Logger = QModManager.Utility.Logger;


namespace QuickSlotsPlus.Patches
{

    [HarmonyPatch(typeof(uGUI_PDA), "OnOpenPDA")]
    class uGUI_PDA_Patch
    {
        private static FieldInfo BaseRaycaster = typeof(uGUI_PDA).GetField("quickSlotsParentRaycaster", BindingFlags.Instance | BindingFlags.NonPublic);
        static void Postfix(uGUI_PDA __instance, PDATab __0)
        {
            uGUI_Block blocker = __instance.GetComponentInChildren<uGUI_Block>();
            MoveTransformUp(blocker.transform);
            
            uGUI_ItemDropTarget[] targets = __instance.GetComponentsInChildren<uGUI_ItemDropTarget>();
            foreach(uGUI_ItemDropTarget target in targets)
            {
                MoveTransformUp(target.transform);
            }
        }

        /*
         * Move UI Blockers up 125px so they're not on top of the quick slots when you open the PDA =/
         * 
         * This should fix quickslot snapping.
         */
        private static void MoveTransformUp(Transform transform)
        {
            var temp = transform.localPosition;
            temp.y += 125f;
            transform.localPosition = temp;
        }
    }

}
