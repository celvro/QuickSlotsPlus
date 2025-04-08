using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using Newtonsoft.Json;

namespace QuickSlotsPlus.Patches
{

    /***
     * Prevent adding new items to the QuickSlot bar.
     */
    [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.BindToEmpty))]
    class QuickSlots_OnAddItem_Patch
    {
        static bool Prefix(InventoryItem item, ref int __result, QuickSlots __instance)
        {
            Mod.logger.LogDebug($"Picked up item, TechType: {item.techType}");

            bool disabled = Mod.Options.disableBindToEmpty && !IntroLifepodDirector.IsActive;
            if (!disabled || ItemHelper.AllowedTechType(item))
            {
                var reservedSlot = ItemHelper.ReservedSlot(item);
                if (reservedSlot > 0 && reservedSlot <= __instance.binding.Length)
                {
                    // Mod.logger.LogDebug($"Reserved item: {item}, slot: {reservedSlot}");
                    __instance.Bind(reservedSlot - 1, item);
                    __result = reservedSlot - 1;
                    return false;
                }
                
                int firstEmpty = FirstEmpty(__instance);
                if (firstEmpty != -1)
                {
                    // Mod.logger.LogDebug($"Binding first empty: {item}, slot: {firstEmpty}");
                    __instance.Bind(firstEmpty, item);
                    __result = firstEmpty;
                    return false;
                }
            }

            __result = -1; // Do nothing
            return false;
        }

        static int FirstEmpty(QuickSlots slots)
        {
            var reserved = ItemHelper.ReservedSlots();

            for (int i = 0; i < slots.binding.Length; i++)
            {
                if (slots.binding[i] == null && !reserved.Contains(i + 1))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    class ItemHelper
    {
        static Dictionary<string, int> allowedTechtypes = new Dictionary<string, int>();

        // Override Mod.Options.disableBindToEmpty for individual slots, reads allowItems.json
        public static bool AllowedTechType(InventoryItem item)
        {
            LoadAllowedTypes();
            return allowedTechtypes.TryGetValue(item.techType.ToString(), out _);
        }

        public static int ReservedSlot(InventoryItem item)
        {
            LoadAllowedTypes();
            if (allowedTechtypes.TryGetValue(item.techType.ToString(), out int slotNum))
            {
                return slotNum;
            }
            else { return -1; }
        }

        public static List<int> ReservedSlots()
        {
            LoadAllowedTypes();
            return new List<int>(allowedTechtypes.Values);
        }

        static void LoadAllowedTypes()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assemblyFolder, "allowItems.json");
            if (File.Exists(path))
            {
                var jsonString = File.ReadAllText(path);
                allowedTechtypes = JsonConvert.DeserializeObject<Dictionary<string, int>>(StripComments(jsonString));
                // Mod.logger.LogDebug("Parsed allowedItems.json");
            }
            else
            {
                // Mod.logger.LogDebug($"File not found: {path}. Skipping.");
            }
        }

        // Remove commented out techtypes before parsing json
        // https://stackoverflow.com/a/31681009
        private static string StripComments(string input)
        {
            input = Regex.Replace(input, @"^\s*//.*$", "", RegexOptions.Multiline);  // removes comments like this
            input = Regex.Replace(input, @"^\s*/\*(\s|\S)*?\*/\s*$", "", RegexOptions.Multiline); /* comments like this */

            return input;
        }
    }
}
