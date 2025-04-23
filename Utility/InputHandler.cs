using UnityEngine;
using UWE;

namespace QuickSlotsPlus.Utility
{
    public class InputHandler : MonoBehaviour
    {
        public void Awake()
        {
            Mod.logger.LogInfo("InputHandler loaded.");
            Player.main.playerModeChanged.AddHandler(gameObject, new Event<Player.Mode>.HandleFunction(this.RedrawSlots));
        }

        public void OnDestroy()
        {
            Mod.logger.LogInfo("InputHandler Destroyed.");
        }

        private void RedrawSlots(Player.Mode _)
        {
            // Mod.logger.LogDebug("Redraw slots because Player mode changed.");
            Mod.RedrawQuickSlots();
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
