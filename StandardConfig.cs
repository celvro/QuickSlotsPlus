using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus
{
    [Menu("QuickSlots+")]
    public class StandardConfig : ConfigFile
    {
        [Slider("QuickSlot Count", 5, 20, DefaultValue = 10), OnChange(nameof(RedrawQuickSlots))]
        public int slotCount = 10;

        [Toggle("Do Not Add New Items to Empty Slots")]
        public bool disableBindToEmpty = true;

        [Toggle("Show HotKey labels"), OnChange(nameof(RedrawQuickSlots))]
        public bool showLabels = true;

        [Slider("Label Size", 5, 30, DefaultValue = 18), OnChange(nameof(RedrawQuickSlots))]
        public int labelSize = 18;

        [Slider("Horizontal Position", -20, 20, DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelXpos = 0;

        [Slider("Vertical Position", -25, 100, DefaultValue = 0), OnChange(nameof(RedrawQuickSlots))]
        public float labelYpos = 0;

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

        /* 
         * Redraw the QuickSlots after user changes preferences in settings menu.
         */
        public static void RedrawQuickSlots()
        {
            Logger.Log(Logger.Level.Debug, "Redraw quick slots due to options change");
            Mod.RedrawQuickSlots();
        }
    }
}
