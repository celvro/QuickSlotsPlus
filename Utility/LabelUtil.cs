using Oculus.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Utility
{
    public static class LabelUtil
    {
        public static string[] slotNames = Enumerable.Range(1, 20).Select(n => "QuickSlot" + n).ToArray();

        public static Dictionary<string, string> CustomLabels = new Dictionary<string, string>();

        public static string getSlotKeyText(int slotId)
        {
            // Hotkeys 1-5
            if (slotId < Player.quickSlotButtonsCount)
            {
                string inputName = GameInput.GetBindingName(GameInput.Button.Slot1 + slotId, GameInput.BindingSet.Primary);
                if (inputName == null)
                {
                    // A HotKey 1-5 is not set
                    return "";
                }

                string input = uGUI.GetDisplayTextForBinding(inputName);
                return KeyCodeToString(input);
            }

            var v = (KeyCode)Mod.Config.GetType().GetField("HotKey" + (slotId + 1)).GetValue(Mod.Config);

            return KeyCodeToString(v);
        }

        public static void LoadCustomLabels()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(assemblyFolder, "customLabels.json");
            if (File.Exists(path))
            {
                var JsonConfig = File.ReadAllText(path);
                Logger.Log(Logger.Level.Debug, "Loaded hotkey label json: \n" + JsonConfig);
                CustomLabels = CreateFromJSON(JsonConfig);
            }
            else
            {
                Logger.Log(Logger.Level.Debug, "Did not find custom label file: " + path);
            }
        }

        private static Dictionary<string, string> CreateFromJSON(string jsonString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public static string KeyCodeToString(KeyCode keyCode)
        {
            if (CustomLabels.TryGetValue(keyCode.ToString(), out string label_1))
            {
                Logger.Log(Logger.Level.Debug, "Found custom label for keycode: " + keyCode.ToString());

                return label_1;
            }
            else if (DefaultLabels.TryGetValue(keyCode, out char label_2))
            {
                Logger.Log(Logger.Level.Debug, "Found default custom label for keycode: " + keyCode.ToString());

                return label_2.ToString();
            }
            else if (keyCode == KeyCode.None)
            {
                Logger.Log(Logger.Level.Debug, "Found KeyCode.None, returning blank string.");

                // Can't enter empty char into dictionary
                return "";
            }

            Logger.Log(Logger.Level.Debug, "Returning unmodified keycode text.");

            return keyCode.ToString();
        }

        public static string KeyCodeToString(string keyCode)
        {
            
            try
            {
                KeyCode kc = (KeyCode)Enum.Parse(typeof(KeyCode), keyCode);
                return KeyCodeToString(kc);

            }
            catch (ArgumentException)
            {
                // KeyCode was not found in Enum, try custom labels
                if (CustomLabels.TryGetValue(keyCode, out string label_1))
                {
                    Logger.Log(Logger.Level.Debug, "Found custom label for keycode: " + keyCode);
                    return label_1;
                }
                return keyCode;
            }
        }

        /* https://gist.github.com/b-cancel/c516990b8b304d47188a7fa8be9a1ad9
         * 
         * You'd think a method exists for this...
         */
        public static readonly Dictionary<KeyCode, char> DefaultLabels = new Dictionary<KeyCode, char>()
        {
          //-------------------------LOGICAL mappings-------------------------

          {KeyCode.A, 'A'},
          {KeyCode.B, 'B'},
          {KeyCode.C, 'C'},
          {KeyCode.D, 'D'},
          {KeyCode.E, 'E'},
          {KeyCode.F, 'F'},
          {KeyCode.G, 'G'},
          {KeyCode.H, 'H'},
          {KeyCode.I, 'I'},
          {KeyCode.J, 'J'},
          {KeyCode.K, 'K'},
          {KeyCode.L, 'L'},
          {KeyCode.M, 'M'},
          {KeyCode.N, 'N'},
          {KeyCode.O, 'O'},
          {KeyCode.P, 'P'},
          {KeyCode.Q, 'Q'},
          {KeyCode.R, 'R'},
          {KeyCode.S, 'S'},
          {KeyCode.T, 'T'},
          {KeyCode.U, 'U'},
          {KeyCode.V, 'V'},
          {KeyCode.W, 'W'},
          {KeyCode.X, 'X'},
          {KeyCode.Y, 'Y'},
          {KeyCode.Z, 'Z'},
  
          //KeyPad Numbers
          {KeyCode.Keypad1, '1'},
          {KeyCode.Keypad2, '2'},
          {KeyCode.Keypad3, '3'},
          {KeyCode.Keypad4, '4'},
          {KeyCode.Keypad5, '5'},
          {KeyCode.Keypad6, '6'},
          {KeyCode.Keypad7, '7'},
          {KeyCode.Keypad8, '8'},
          {KeyCode.Keypad9, '9'},
          {KeyCode.Keypad0, '0'},
  
          //Other Symbols
          {KeyCode.Exclaim, '!'},
          {KeyCode.At, '@'},
          {KeyCode.Hash, '#'},
          {KeyCode.Dollar, '$'},
          {KeyCode.Caret, '^'},
          {KeyCode.Ampersand, '&'},
          {KeyCode.Asterisk, '*'},
          {KeyCode.LeftParen, '('},
          {KeyCode.RightParen, ')'},
          {KeyCode.Plus, '+'},
          {KeyCode.Comma, ','},
          {KeyCode.Minus, '-'},
          {KeyCode.Period, '.'},
          {KeyCode.Slash, '/'},
          {KeyCode.Colon, ':'},
          {KeyCode.Semicolon, ';'},
          {KeyCode.Less, '<'},
          {KeyCode.Equals, '='},
          {KeyCode.Greater, '>'},
          {KeyCode.Question, '?'},
          {KeyCode.LeftBracket, '['},
          {KeyCode.Backslash, '\\' },
          {KeyCode.RightBracket, ']'},
          {KeyCode.Underscore, '_'},
          {KeyCode.BackQuote, '`'},
          {KeyCode.Quote, '\'' },
          {KeyCode.DoubleQuote, '"'},
          
          //-------------------------NON-LOGICAL mappings-------------------------
          
          
          //Alpha Numbers
          {KeyCode.Alpha1, '1'},
          {KeyCode.Alpha2, '2'},
          {KeyCode.Alpha3, '3'},
          {KeyCode.Alpha4, '4'},
          {KeyCode.Alpha5, '5'},
          {KeyCode.Alpha6, '6'},
          {KeyCode.Alpha7, '7'},
          {KeyCode.Alpha8, '8'},
          {KeyCode.Alpha9, '9'},
          {KeyCode.Alpha0, '0'},
          
          {KeyCode.KeypadPeriod, '.'},
          {KeyCode.KeypadDivide, '/'},
          {KeyCode.KeypadMultiply, '*'},
          {KeyCode.KeypadMinus, '-'},
          {KeyCode.KeypadPlus, '+'},
          {KeyCode.KeypadEquals, '='},

  
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
    }
}
