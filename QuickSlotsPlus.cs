using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using QuickSlotsPlus.Patches;

namespace QuickSlotsPlus
{

    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class QuickSlotsPlus : BaseUnityPlugin
    {
        internal static StandardConfig Options { get; } = OptionsPanelHandler.RegisterModOptions<StandardConfig>();

        private const string myGUID = "com.celvro.subnautica.quickslotsplus";
        private const string pluginName = "Quick Slots Plus";
        private const string versionString = "3.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("QuickSlots+");

        public void Awake()
        {
            LanguageHandler.RegisterLocalizationFolder();
            harmony.PatchAll();
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

        public void Update()
        {
            if (!GameInput.IsInitialized || Inventory.main == null)
            {
                return;
            }

            if (GameInput.GetButtonDown(Slot6)) { Inventory.main.quickSlots.SlotKeyDown(5); }
            if (GameInput.GetButtonDown(Slot7)) { Inventory.main.quickSlots.SlotKeyDown(6); }
            if (GameInput.GetButtonDown(Slot8)) { Inventory.main.quickSlots.SlotKeyDown(7); }
            if (GameInput.GetButtonDown(Slot9)) { Inventory.main.quickSlots.SlotKeyDown(8); }
            if (GameInput.GetButtonDown(Slot10)) { Inventory.main.quickSlots.SlotKeyDown(9); }
            if (GameInput.GetButtonDown(Slot11)) { Inventory.main.quickSlots.SlotKeyDown(10); }
            if (GameInput.GetButtonDown(Slot12)) { Inventory.main.quickSlots.SlotKeyDown(11); }
            if (GameInput.GetButtonDown(Slot13)) { Inventory.main.quickSlots.SlotKeyDown(12); }
            if (GameInput.GetButtonDown(Slot14)) { Inventory.main.quickSlots.SlotKeyDown(13); }
            if (GameInput.GetButtonDown(Slot15)) { Inventory.main.quickSlots.SlotKeyDown(14); }
            if (GameInput.GetButtonDown(Slot16)) { Inventory.main.quickSlots.SlotKeyDown(15); }
            if (GameInput.GetButtonDown(Slot17)) { Inventory.main.quickSlots.SlotKeyDown(16); }
            if (GameInput.GetButtonDown(Slot18)) { Inventory.main.quickSlots.SlotKeyDown(17); }
            if (GameInput.GetButtonDown(Slot19)) { Inventory.main.quickSlots.SlotKeyDown(18); }
            if (GameInput.GetButtonDown(Slot20)) { Inventory.main.quickSlots.SlotKeyDown(19); }
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