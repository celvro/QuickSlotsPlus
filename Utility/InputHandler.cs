using UnityEngine;
using UWE;

namespace QuickSlotsPlus.Utility
{
    public class InputHandler : MonoBehaviour
    {
        private void Awake()
        {
            Mod.logger.LogInfo("InputHandler loaded.");
            Player.main.playerModeChanged.AddHandler(gameObject, new Event<Player.Mode>.HandleFunction(this.RedrawSlots));
        }

        private void RedrawSlots(Player.Mode _)
        {
            Mod.RedrawQuickSlots();
        }

        private void OnDestroy()
        {
            Mod.logger.LogInfo("InputHandler Destroyed.");
        }

        public void Update()
        {
            for(var i = 6; i <= Mod.Options.slotCount; i++)
            {
                if(GetKeyDownForSlot(i))
                {
                    Inventory.main.quickSlots.SlotKeyDown(i - 1);
                }
            }
        }

        public bool GetKeyDownForSlot(int slotId)
        {
            return Input.GetKeyDown(getSlotKey(slotId));
        }

        public static KeyCode getSlotKey(int slotId)
        {

            return (KeyCode)Mod.Options.GetType().GetField("HotKey" + slotId).GetValue(Mod.Options);
        }
    }
}
