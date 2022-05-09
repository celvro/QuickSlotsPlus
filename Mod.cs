using HarmonyLib;
using SMLHelper.V2.Handlers;
using QModManager.API.ModLoading;
using UnityEngine;
using Logger = QModManager.Utility.Logger;
using System.Reflection;
using QuickSlotsPlus.Patches;

namespace QuickSlotsPlus
{

    [QModCore]
    public static class Mod
    {
        internal static StandardConfig Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<StandardConfig>();

        // Stores the user's selected item order
        private static FieldInfo InventoryItemsBinding = typeof(QuickSlots).GetField("binding", BindingFlags.NonPublic | BindingFlags.Instance);

        [QModPatch]
        public static void Load()
        {
            Config.Load();
            Logger.Log(Logger.Level.Info, "Loaded Config.");

            Harmony harmony = new Harmony("com.celvro.subnautica.quickslotsplus");
            harmony.PatchAll();
        }

        /*
         * Redraw QuickSlot items while game is currently running. Also destroys and redraws HotKey labels if enabled.
         */
        public static void RedrawQuickSlots()
        {
            if (Inventory.main == null) return;

            // Saving the binding allows us to restore QuickSlot items
            var oldSlots = Inventory.main.quickSlots;
            var oldBinding = (InventoryItem[])InventoryItemsBinding.GetValue(oldSlots);

            // In case QuickSlot size is reduced and held item no longer fits on bar
            oldSlots.DeselectImmediate();

            // Destroy and then redraw Quickslots (including HotKey labels)
            Inventory_Awake_Patch.Postfix(Inventory.main);

            // Need a large enough array in case QuickSlot size is increased
            InventoryItem[] newBinding = new InventoryItem[20];
            for (var i = 0; i < oldBinding.Length; i++)
            {
                newBinding[i] = oldBinding[i];
            }
            // Restore QuickSlot selections
            InventoryItemsBinding.SetValue(Inventory.main.quickSlots, newBinding);
        }
    }
}