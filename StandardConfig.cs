using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine.UI;

namespace QuickSlotsPlus
{
    [Menu("QuickSlots+")]
    public class StandardConfig : ConfigFile
    {
        [Slider(0, 20, DefaultValue = 10, LabelLanguageId = "SlotCount", TooltipLanguageId = "SlotCount_Tooltip"), OnChange(nameof(RedrawQuickSlots))]
        public int slotCount = 10;

        [Toggle(LabelLanguageId = "DisableBindToEmpty", TooltipLanguageId = "DisableBindToEmpty_Tooltip")]
        public bool disableBindToEmpty = true;

        [Toggle(LabelLanguageId = "ShowHotkeyLabels"), OnChange(nameof(RedrawQuickSlots))]
        public bool showLabels = true;

        [Toggle(LabelLanguageId = "ShowLabelsWhilePiloting", TooltipLanguageId = "ShowLabelsWhilePiloting_Tooltip")]
        public bool showLabelsWhilePiloting = false;

#if BELOWZERO
        [ColorPicker("Label Color", Advanced = true), OnChange(nameof(RedrawQuickSlots))]
        public Color color = Color.white;
#endif

        [Slider(10, 80, LabelLanguageId = "LabelSize", DefaultValue = 20), OnChange(nameof(RedrawQuickSlots))]
        public float labelSize = 20;

        [Slider(-30, 30, LabelLanguageId = "LabelHorizontal", DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelXpos = 0;

        [Slider(-20, 100, LabelLanguageId = "LabelVertical", DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
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
