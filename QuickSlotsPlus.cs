using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using QuickSlotsPlus.Patches;
using QuickSlotsPlus.Utility;

namespace QuickSlotsPlus
{

    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class QuickSlotsPlus : BaseUnityPlugin
    {
        internal static StandardConfig Options { get; } = OptionsPanelHandler.RegisterModOptions<StandardConfig>();

        private const string myGUID = "com.celvro.subnautica.quickslotsplus";
        private const string pluginName = "Quick Slots Plus";
        private const string versionString = "3.1.0";

        private Harmony harmony;

        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("QuickSlots+");

        public void Awake()
        {
            LanguageHandler.RegisterLocalizationFolder();
            harmony = new Harmony(myGUID);
            harmony.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

#if SUBNAUTICA
        public static GameInput.Button Slot6 = EnumHandler.AddEntry<GameInput.Button>("Slot6")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Key6)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot7 = EnumHandler.AddEntry<GameInput.Button>("Slot7")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Key7)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot8 = EnumHandler.AddEntry<GameInput.Button>("Slot8")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Key8)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot9 = EnumHandler.AddEntry<GameInput.Button>("Slot9")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Key9)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot10 = EnumHandler.AddEntry<GameInput.Button>("Slot10")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Key0)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot11 = EnumHandler.AddEntry<GameInput.Button>("Slot11")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Minus)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot12 = EnumHandler.AddEntry<GameInput.Button>("Slot12")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Equals)
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot13 = EnumHandler.AddEntry<GameInput.Button>("Slot13")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot14 = EnumHandler.AddEntry<GameInput.Button>("Slot14")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot15 = EnumHandler.AddEntry<GameInput.Button>("Slot15")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot16 = EnumHandler.AddEntry<GameInput.Button>("Slot16")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot17 = EnumHandler.AddEntry<GameInput.Button>("Slot17")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot18 = EnumHandler.AddEntry<GameInput.Button>("Slot18")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot19 = EnumHandler.AddEntry<GameInput.Button>("Slot19")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        public static GameInput.Button Slot20 = EnumHandler.AddEntry<GameInput.Button>("Slot20")
            .CreateInput()
            .SetBindable()
            .WithKeyboardBinding("None")
            .AvoidConflicts(GameInput.Device.Keyboard);

        // Index 0 is slot 6, so ExtraSlotButtons[i] drives quick slot index i + 5.
        // Declared after the Slot6..Slot20 fields so their initializers have already run.
        internal static readonly GameInput.Button[] ExtraSlotButtons =
        {
            Slot6, Slot7, Slot8, Slot9, Slot10,
            Slot11, Slot12, Slot13, Slot14, Slot15,
            Slot16, Slot17, Slot18, Slot19, Slot20
        };

        public void Update()
        {
            if (!GameInput.IsInitialized || Inventory.main == null)
            {
                return;
            }

            // TEMPORARY, remove along with Utility/Diagnostics.cs.
            Diagnostics.Tick();
            Diagnostics.LogVanillaSlotPresses();

            QuickSlots quickSlots = Inventory.main.quickSlots;

            for (var i = 0; i < ExtraSlotButtons.Length; i++)
            {
                if (!GameInput.GetButtonDown(ExtraSlotButtons[i]))
                {
                    continue;
                }

                int slotIndex = i + 5;

                Diagnostics.LogSlotKeyDown(ExtraSlotButtons[i], slotIndex, quickSlots);
                quickSlots.SlotKeyDown(slotIndex);
                Diagnostics.LogSlotKeyDownResult(slotIndex, quickSlots);
            }
        }
#endif
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