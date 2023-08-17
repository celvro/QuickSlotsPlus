using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace QuickSlotsPlus.Utility
{
    public static class LabelUtil
    {
        public static string[] slotNames = Enumerable.Range(1, 20).Select(n => "QuickSlot" + n).ToArray();

        public static Dictionary<string, string> CustomLabels = new Dictionary<string, string>();

        public static string getSlotKeyText(int slotId)
        {
            string bindingName;
            // Hotkeys 1-5
            if (slotId < Player.quickSlotButtonsCount)
            {
                bindingName = GameInput.GetBindingName(GameInput.Button.Slot1 + slotId, GameInput.BindingSet.Primary);
                if (bindingName == null)
                {
                    // A HotKey 1-5 is not set
                    return "";
                }
            }
            else
            {
                KeyCode keyCode = (KeyCode)Mod.Options.GetType().GetField("HotKey" + (slotId + 1)).GetValue(Mod.Options);
                bindingName = GameInput.GetInputName(GameInput.GetKeyCodeAsInputName(keyCode));
            }

            string input = uGUI.GetDisplayTextForBinding(bindingName);
            return KeyCodeToString(input);
        }

        public static void LoadCustomLabels()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assemblyFolder, "customLabels.json");
            if (File.Exists(path))
            {
                var JsonConfig = File.ReadAllText(path);
                Mod.logger.LogDebug("Loaded hotkey label json: \n" + JsonConfig);
                CustomLabels = CreateFromJSON(JsonConfig);
            }
            else
            {
                Mod.logger.LogDebug("Did not find custom label file: " + path);
            }
        }

        private static Dictionary<string, string> CreateFromJSON(string jsonString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public static string KeyCodeToString(string keyCode)
        {
            // KeyCode was not found in Enum, try custom labels
            if (CustomLabels.TryGetValue(keyCode, out string label_1))
            {
                Mod.logger.LogDebug($"Found custom label {label_1} for keycode {keyCode}");
                return label_1;
            }
            else if (DefaultLabels.TryGetValue(keyCode, out char label_2))
            {
                Mod.logger.LogDebug($"Found default custom label {label_2} for keycode {keyCode}");
                return label_2.ToString();
            }
            return keyCode;
        }

        /* https://gist.github.com/b-cancel/c516990b8b304d47188a7fa8be9a1ad9
         * 
         * You'd think a method exists for this...
         */
        public static readonly Dictionary<string, char> DefaultLabels = new Dictionary<string, char>()
        {
          //-------------------------LOGICAL mappings-------------------------
          {"None", ' ' },
  
          //KeyPad Numbers
          {"Keypad1", '1'},
          {"Keypad2", '2'},
          {"Keypad3", '3'},
          {"Keypad4", '4'},
          {"Keypad5", '5'},
          {"Keypad6", '6'},
          {"Keypad7", '7'},
          {"Keypad8", '8'},
          {"Keypad9", '9'},
          {"Keypad0", '0'},
  
          //Other Symbols
          {"Exclaim", '!'},
          {"At", '@'},
          {"Hash", '#'},
          {"Dollar", '$'},
          {"Caret", '^'},
          {"Ampersand", '&'},
          {"Asterisk", '*'},
          {"LeftParen", '('},
          {"RightParen", ')'},
          {"Plus", '+'},
          {"Comma", ','},
          {"Minus", '-'},
          {"Period", '.'},
          {"Slash", '/'},
          {"Colon", ':'},
          {"Semicolon", ';'},
          {"Less", '<'},
          {"Equals", '='},
          {"Greater", '>'},
          {"Question", '?'},
          {"LeftBracket", '['},
          {"Backslash", '\\' },
          {"RightBracket", ']'},
          {"Underscore", '_'},
          {"BackQuote", '`'},
          {"Quote", '\'' },
          {"DoubleQuote", '"'},
          
          //-------------------------NON-LOGICAL mappings-------------------------
          
          
          //Alpha Numbers
          {"Alpha1", '1'},
          {"Alpha2", '2'},
          {"Alpha3", '3'},
          {"Alpha4", '4'},
          {"Alpha5", '5'},
          {"Alpha6", '6'},
          {"Alpha7", '7'},
          {"Alpha8", '8'},
          {"Alpha9", '9'},
          {"Alpha0", '0'},
          
          {"KeypadPeriod", '.'},
          {"KeypadDivide", '/'},
          {"KeypadMultiply", '*'},
          {"KeypadMinus", '-'},
          {"KeypadPlus", '+'},
          {"KeypadEquals", '='},

  
          //-------------------------KEYCODES with NO CHARACER KEY-------------------------
  
          //-----KeyCodes without Logical Mappings
          //-Anything above "KeyCode.Space" in Unity's Documentation (9 KeyCodes)
          //-Anything between "KeyCode.UpArrow" and "KeyCode.F15" in Unity's Documentation (24 KeyCodes)
          //-Anything Below "KeyCode.Numlock" in Unity's Documentation [(28 KeyCodes) + (9 * 20 = 180 JoyStickCodes) = 208 KeyCodes]
  
          //-------------------------other-------------------------

          //-----KeyCodes that are inaccesible for some reason
          //{'~', KeyCode.tilde},
          //{'{', KeyCode.LeftCurlyBrace}, 
          //{'}', KeyCode.RightCurlyBrace}, 
          //{'|', KeyCode.Line},   
          //{'%', KeyCode.percent},
        };

        public static void DrawLabels(uGUI_QuickSlots quickSlots)
        {
            uGUI_ItemIcon[] icons = quickSlots.icons;
            if (!ShouldShowLabels(quickSlots) || icons == null)
            {
                return;
            }

            // Reading in "customLabels.json", keep it outside the loop
            LoadCustomLabels();

            TextMeshProUGUI textPrefab = GetTextPrefab();
            if (textPrefab == null)
            {
                Mod.logger.LogDebug("Could not find text prefab.");
                return;
            }

            for (var i = 0; i < icons.Length; i++)
            {
                uGUI_ItemIcon itemIcon = icons[i];
                var index = i;
                // Fix for slot labels since first 2 Prawn slots are right and left click
                if (Player.main.inExosuit && PDAClosed())
                {
                    /*
                     * LeftHand  = -4,
                     * RightHand = -3,
                     * CycleNext = -2,
                     * CyclePrev = -1,
                     * Slot1     = 0
                     */
                    if (index < 2)
                    {
                        index -= 4;
                    }
                    else
                    {
                        index -= 2;
                    }
                }
                CreateNewText(textPrefab, itemIcon.transform, index);
            }
        }

        private static bool PDAClosed()
        {
            PDA.State state = Player.main.pda.state;
            return state == PDA.State.Closed || state == PDA.State.Closing;
        }

        private static TextMeshProUGUI GetTextPrefab()
        {
            return Object.FindObjectOfType<HandReticle>()?.compTextHand;
        }

        private static bool ShouldShowLabels(uGUI_QuickSlots quickSlots)
        {
            if (quickSlots == null || !Mod.Options.showLabels)
            {
                return false;
            }

            Player player = Player.main;
            return player != null && (player.GetMode() == Player.Mode.Normal || Mod.Options.showLabelsWhilePiloting);
        }

        // Used dnspy to rip the bones from RandyKnapp's version: https://www.nexusmods.com/subnautica/mods/14
        private static TextMeshProUGUI CreateNewText(TextMeshProUGUI prefab, Transform parent, int index = -1)
        {
            TextMeshProUGUI text = Object.Instantiate(prefab);
            text.gameObject.layer = parent.gameObject.layer;
            text.gameObject.name = "QuickSlotText" + ((index >= 0) ? index.ToString() : "");
            text.transform.SetParent(parent, false);
            text.transform.localScale = new Vector3(1f, 1f, 1f);
            text.gameObject.SetActive(true);
            text.enabled = true;
            text.text = getSlotKeyText(index);
            text.alignment = TextAlignmentOptions.Center;
            RectTransformExtensions.SetParams(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            text.rectTransform.SetSizeWithCurrentAnchors(0, 100f);
            text.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, 100f);
            text.raycastTarget = false;

            // **** Set config options ****
            text.fontSize = (int)Mod.Options.labelSize;

            float xPos = Mod.Options.labelXpos;
            float yPos = Mod.Options.labelYpos - 36f; // Subtract 36 to make default position be below the icons
            text.rectTransform.anchoredPosition = new Vector3(xPos, yPos);

            return text;
        }
    }
}
