using UnityEngine;

namespace QuickSlotsPlus.Utility
{
    /***
     * Polls the hotkeys for the extra quick slots (6 and up).
     *
     * This lives on the Inventory GameObject rather than on the BepInEx plugin component. Unity disables and
     * destroys the plugin component during the first scene load, while the game is still running, so an
     * Update() there stops being called before the player ever reaches the world: scrolling, labels and the
     * extra slots themselves all keep working, which makes it look like only the keybinds are broken.
     *
     * Below Zero already worked this way; Subnautica used to poll from the plugin component instead.
     */
    public class InputHandler : MonoBehaviour
    {
        public void Update()
        {
            if (Inventory.main == null)
            {
                return;
            }

            QuickSlots quickSlots = Inventory.main.quickSlots;
            if (quickSlots == null)
            {
                return;
            }

            for (var i = 6; i <= QuickSlotsPlus.Options.slotCount; i++)
            {
                if (!GetKeyDownForSlot(i))
                {
                    continue;
                }

                quickSlots.SlotKeyDown(i - 1);
            }
        }

#if SUBNAUTICA
        public bool GetKeyDownForSlot(int slotId)
        {
            return GameInput.IsInitialized && GameInput.GetButtonDown(QuickSlotsPlus.ExtraSlotButtons[slotId - 6]);
        }
#else
        public bool GetKeyDownForSlot(int slotId)
        {
            return Input.GetKeyDown(GetSlotKey(slotId));
        }

        public static KeyCode GetSlotKey(int slotId)
        {
            return (KeyCode)QuickSlotsPlus.Options.GetType().GetField("HotKey" + slotId).GetValue(QuickSlotsPlus.Options);
        }
#endif
    }
}
