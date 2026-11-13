using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Newtonsoft.Json;
using QuickSlotsPlus.Patches;
using QuickSlotsPlus.Utility;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            RegisterLocalization();
            harmony = new Harmony(myGUID);
            harmony.PatchAll();
        }

        /*
         * Deliberately does NOT use Nautilus's LanguageHandler.RegisterLocalizationFolder().
         *
         * That helper locates the Localization folder from Assembly.GetCallingAssembly(), falling back to
         * the first non-Nautilus frame of a fresh StackTrace. Neither survives the native Unity -> Awake()
         * boundary here: it resolved to the game's own assembly folder and logged
         *
         *   [Error : Nautilus] Directory '...\Subnautica_Data\Managed\Localization' does not exist.
         *                      Skipping localization registration.
         *
         * so not one line was ever registered and every option label fell back to its field name - in every
         * language, English included. Resolving the folder against THIS assembly is deterministic, so load
         * the files here and hand the parsed tables to Nautilus directly.
         */
        private static void RegisterLocalization()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folder = Path.Combine(assemblyFolder, "Localization");

            if (!Directory.Exists(folder))
            {
                logger.LogWarning($"No Localization folder at {folder}. Option labels will show their field names.");
                return;
            }

            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                string language = Path.GetFileNameWithoutExtension(file);
                try
                {
                    var lines = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                    if (lines == null || lines.Count == 0)
                    {
                        logger.LogWarning($"Localization file {language}.json is empty. Skipping.");
                        continue;
                    }

                    LanguageHandler.RegisterLocalization(language, lines);
                }
                catch (JsonReaderException ex)
                {
                    ErrorMessage.AddError($"Error reading Localization/{language}.json\n{ex.Message}");
                    logger.LogError($"Error reading Localization/{language}.json\n{ex.Message}");
                }
            }
        }

        /*
         * Deliberately does NOT unpatch.
         *
         * Unity disables and destroys this component during the first scene load, while the game is still
         * running, long before Inventory.Awake. Unpatching there tore out every patch for the rest of the
         * session: the quickbar stayed at the vanilla 5 slots because Inventory_Awake_Patch never ran, hotkeys
         * 6-20 died with Update(), and QuickSlots went back to the vanilla 5-length slotNames array, so any
         * selection past slot 5 threw IndexOutOfRangeException from Inventory.UpdateContainers every frame -
         * the rapid equip/unequip flicker that carries on into the pause menu.
         *
         * The patches have to outlive the component. There is nothing to clean up at shutdown anyway, since
         * the process is going away with them.
         */
        public void OnDestroy()
        {
            // TEMPORARY, remove along with Utility/Diagnostics.cs.
#if SUBNAUTICA
            QuickSlotsPlus.logger.LogWarning($"[diag] OnDestroy (patches deliberately left in place). {Diagnostics.DescribeCaller()}");
#endif
        }

        public void OnDisable()
        {
            // TEMPORARY, remove along with Utility/Diagnostics.cs.
#if SUBNAUTICA
            QuickSlotsPlus.logger.LogWarning("[diag] OnDisable: the plugin component was disabled, Update() will stop running");
#endif
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