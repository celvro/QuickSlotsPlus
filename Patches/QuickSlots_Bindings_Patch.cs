using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using SMLHelper.V2.Utility;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Patches
{

    /**
     * When subnautica saves the game it skips null items in the quickslots. Patching the save method manually to include empty items.
     */
    class QuickSlots_Bindings_Patch
    {
        private static FieldInfo QuickSlots_binding = typeof(QuickSlots).GetField("binding", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo QuickSlots_container = typeof(QuickSlots).GetField("container", BindingFlags.Instance | BindingFlags.NonPublic);
        private static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /*
         * Create a wrapper class to serialize/deserialize the json.
         */
        class BindingWrapper
        {
            public BindingWrapper()
            {
                bindings = new string[20];
            }
            public BindingWrapper(string[] bind) => bindings = bind;
            public string[] bindings;
        }

        [HarmonyPatch(typeof(QuickSlots), "SaveBinding")]
        class QuickSlots_SaveBinding_Patch
        {
            static bool Prefix(ref string[] __result)
            {
                __result = new string[20];

                var quickSlots = Inventory.main.quickSlots;
                Logger.Log(Logger.Level.Debug, "Got Inventory.main.quickslots");

                if (quickSlots != null)
                {
                    var binding = (InventoryItem[])QuickSlots_binding.GetValue(quickSlots);
                    Logger.Log(Logger.Level.Debug, "Got binding values");

                    for (int i = 0; i < binding.Length; i++)
                    {
                        __result[i] = "empty";

                        InventoryItem inventoryItem = binding[i];
                        if (inventoryItem != null && !(inventoryItem.item == null))
                        {
                            UniqueIdentifier component = inventoryItem.item.GetComponent<UniqueIdentifier>();
                            if (!(component == null))
                            {
                                __result[i] = component.Id;
                            }
                        }
                    }
                    Logger.Log(Logger.Level.Debug, "== Save bindings ==");
                    for (int i = 0; i < __result.Length; i++)
                    {
                        Logger.Log(Logger.Level.Debug, "uid: " + __result[i]);
                    }

                    var path = Path.Combine(assemblyFolder, "quickslotBindings.json");
                    JsonUtils.Save(new BindingWrapper(__result), path);
                }
                return true; // this skips the original call
            }

        }

        [HarmonyPatch(typeof(QuickSlots), "RestoreBinding")]
        class QuickSlots_RestoreBinding_Patch
        {
            static bool Prefix(QuickSlots __instance, ref string[] uids)
            {
                var path = Path.Combine(assemblyFolder, "quickslotBindings.json");
                if(File.Exists(path))
                {
                    var wrapper = JsonUtils.Load<BindingWrapper>(path);
                    uids = wrapper.bindings;
                }

                Logger.Log(Logger.Level.Debug, "== Load bindings ==");
                for (int i = 0; i < uids.Length; i++)
                {
                    Logger.Log(Logger.Level.Debug, "uid: " + uids[i]);
                }
                var quickSlots = Inventory.main.quickSlots;
                var binding = (InventoryItem[])QuickSlots_binding.GetValue(quickSlots);

                for (int i = 0; i < binding.Length; i++)
                {
                    __instance.Unbind(i);
                }
                var qsContainer = (IEnumerable<InventoryItem>)QuickSlots_container.GetValue(__instance);
                foreach (InventoryItem inventoryItem in qsContainer)
                {
                    UniqueIdentifier component = inventoryItem.item.GetComponent<UniqueIdentifier>();
                    if (!(component == null))
                    {
                        int num = Mathf.Min(uids.Length, binding.Length);
                        for (int j = 0; j < num; j++)
                        {
                            if (uids[j] == component.Id)
                            {
                                __instance.Bind(j, inventoryItem);
                                break;
                            }
                        }
                    }
                }
                return true;
            }

        }
    }

}
