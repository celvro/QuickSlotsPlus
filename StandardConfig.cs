using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSlotsPlus
{
    /*
     * Labels are passed as Label = "<language id>", deliberately NOT as LabelLanguageId.
     *
     * The game never rebuilds the options panel when the language changes. Every row carries a
     * TranslationLiveUpdate component whose translationKey is the label string it was created with, and
     * on Language.OnLanguageChanged it re-runs Language.main.Get(translationKey). Nautilus resolves a
     * LabelLanguageId itself and hands the game the translated text, so the key becomes e.g. "Slot Count",
     * the re-lookup finds nothing, and the row stays in the old language until the game restarts. Handing
     * the game the id as the label makes it do the lookup, and the live update, itself.
     *
     * Tooltips have no such component. Nautilus resolves TooltipLanguageId each time the row is created,
     * so they follow the language once the options panel has been closed and reopened.
     */
    [Menu("QuickSlots+")]
    public class StandardConfig : ConfigFile
    {
        [Slider(0, 20, DefaultValue = 10, Label = "SlotCount", TooltipLanguageId = "SlotCount_Tooltip"), OnChange(nameof(RedrawQuickSlots))]
        public int slotCount = 10;

        [Toggle(Label = "DisableBindToEmpty", TooltipLanguageId = "DisableBindToEmpty_Tooltip")]
        public bool disableBindToEmpty = true;

        [Toggle(Label = "ShowHotkeyLabels"), OnChange(nameof(RedrawQuickSlots))]
        public bool showLabels = true;

        [Toggle(Label = "ShowLabelsWhilePiloting", TooltipLanguageId = "ShowLabelsWhilePiloting_Tooltip")]
        public bool showLabelsWhilePiloting = false;

#if BELOWZERO
        [ColorPicker("Label Color", Advanced = true), OnChange(nameof(RedrawQuickSlots))]
        public Color color = Color.white;
#endif

        [Slider(10, 80, Label = "LabelSize", DefaultValue = 20), OnChange(nameof(RedrawQuickSlots))]
        public float labelSize = 20;

        [Slider(-30, 30, Label = "LabelHorizontal", DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelXpos = 0;

        [Slider(-20, 100, Label = "LabelVertical", DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelYpos = 0;

#if BELOWZERO
        [Keybind("Slot 6"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey6 = KeyCode.Alpha6;

        [Keybind("Slot 7"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey7 = KeyCode.Alpha7;

        [Keybind("Slot 8"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey8 = KeyCode.Alpha8;

        [Keybind("Slot 9"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey9 = KeyCode.Alpha9;

        [Keybind("Slot 10"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey10 = KeyCode.Alpha0;

        [Keybind("Slot 11"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey11 = KeyCode.Minus;

        [Keybind("Slot 12"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey12 = KeyCode.Equals;

        [Keybind("Slot 13"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey13;

        [Keybind("Slot 14"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey14;

        [Keybind("Slot 15"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey15;

        [Keybind("Slot 16"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey16;

        [Keybind("Slot 17"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey17;

        [Keybind("Slot 18"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey18;

        [Keybind("Slot 19"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey19;

        [Keybind("Slot 20"), OnChange(nameof(RedrawQuickSlots))]
        public KeyCode HotKey20;
#endif

        /* 
         * Redraw the QuickSlots after user changes preferences in settings menu.
         */
        public static void RedrawQuickSlots()
        {
            QuickSlotsPlus.logger.LogDebug("Redraw quick slots due to options change");
            QuickSlotsPlus.RedrawQuickSlots();
        }
    }
}
