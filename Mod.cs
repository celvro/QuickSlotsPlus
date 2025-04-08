using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using QuickSlotsPlus.Patches;

namespace QuickSlotsPlus
{

    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Mod : BaseUnityPlugin
    {
        internal static StandardConfig Options { get; } = OptionsPanelHandler.RegisterModOptions<StandardConfig>();

        private const string myGUID = "com.celvro.subnautica.quickslotsplus";
        private const string pluginName = "Quick Slots Plus";
        private const string versionString = "2.2.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("QuickSlots+");

        private void Awake()
        {
            Options.Load();
            harmony.PatchAll();
            logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
        }

        /*
         * Redraw QuickSlot items while game is currently running. Also destroys and redraws HotKey labels if enabled.
         */
        public static void RedrawQuickSlots()
        {
            if (Inventory.main == null) return;

            // Saving the binding allows us to restore QuickSlot items
            var oldSlots = Inventory.main.quickSlots;
            var oldBinding = oldSlots.binding;

            // In case QuickSlot size is reduced and held item no longer fits on bar
            oldSlots.DeselectImmediate();

            // Destroy and then redraw Quickslots (including HotKey labels)
            Inventory_Awake_Patch.Postfix(Inventory.main);

            // Restore QuickSlot selections so items don't get cleared off the bar
            InventoryItem[] newBinding = new InventoryItem[Options.slotCount];
            for (var i = 0; i < oldBinding.Length && i < Options.slotCount; i++)
            {
                newBinding[i] = oldBinding[i];
            }

            Inventory.main.quickSlots.binding = newBinding;
        }
    }
}