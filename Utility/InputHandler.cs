using UnityEngine;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace QuickSlotsPlus.Utility
{
    public class InputHandler : MonoBehaviour
    {
        private void Awake()
        {
            Logger.Log(Logger.Level.Info, "InputHandler loaded.");
            Player.main.playerModeChanged.AddHandler(gameObject, new Event<Player.Mode>.HandleFunction(this.RedrawSlots));
        }

        private void OnDestroy()
        {
            Logger.Log(Logger.Level.Info, "InputHandler Destroyed.");
        }

        private void RedrawSlots(Player.Mode _)
        {
            Mod.RedrawQuickSlots();
        }

        public void Update()
        {
            for(var i = 6; i <= Mod.Config.slotCount; i++)
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

            return (KeyCode)Mod.Config.GetType().GetField("HotKey" + slotId).GetValue(Mod.Config);
        }
    }
}
