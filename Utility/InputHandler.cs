#if BELOWZERO
using UnityEngine;

namespace QuickSlotsPlus.Utility
{
    public class InputHandler : MonoBehaviour
    {
        public void Update()
        {
            for (var i = 6; i <= QuickSlotsPlus.Options.slotCount; i++)
            {
                if (GetKeyDownForSlot(i))
                {
                    Inventory.main.quickSlots.SlotKeyDown(i - 1);
                }
            }
        }

        public bool GetKeyDownForSlot(int slotId)
        {
            return Input.GetKeyDown(GetSlotKey(slotId));
        }

        public static KeyCode GetSlotKey(int slotId)
        {
            return (KeyCode)QuickSlotsPlus.Options.GetType().GetField("HotKey" + slotId).GetValue(QuickSlotsPlus.Options);
        }
    }
}
#endif
