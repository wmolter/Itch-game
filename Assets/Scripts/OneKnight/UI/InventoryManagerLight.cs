using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace OneKnight.UI {
    public class InventoryManagerLight : MonoBehaviour {

        Inventory inventory;
        public Inventory Inventory { get { return inventory; } }
        public ReusableElementManager elementManager;

        public bool ValidateOnChildChange;
        public List<string> categories;

        // Use this for initialization

        public void SetInventory(Inventory toDisplay) {
            if(inventory != null)
                inventory.OnChange -= OnInventoryChanged;
            inventory = toDisplay;
            if(inventory != null)
                inventory.OnChange += OnInventoryChanged;
            Validate();
        }

        public void AddListener(UnityAction<ItemSlot, PointerEventData> listener) {
            foreach(GameObject el in elementManager) {
                el.GetComponent<ItemDisplayLight>().OnClick += listener;
            }
        }

        public void RemoveListener(UnityAction<ItemSlot, PointerEventData> listener) {
            foreach(GameObject el in elementManager) {
                el.GetComponent<ItemDisplayLight>().OnClick -= listener;
            }
        }

        private void OnEnable() {
            Validate();
        }


        public void Validate() {
            if(Inventory != null) {
                Debug.Log("Called parent validate, and inventory slots are: " + Inventory.Capacity);
                int index = 0;
                foreach(ItemSlot slotInv in Inventory) {
                    if(Filter(slotInv)) {
                        InitSlot(slotInv, index);
                        index++;
                    }
                }
                Debug.Log("Disabled from: " + index);
                elementManager.DisableFrom(index);
            } else {
                elementManager.DisableFrom(0);
            }
        }

        protected bool Filter(ItemSlot slot) {
            if(categories == null ||categories.Count == 0)
                return true;
            if(slot == null || slot.Item == null)
                return false;
            bool result = categories.Contains(slot.Item.Category);
            return result;
        }

        protected virtual void InitSlot(ItemSlot slotInv, int index) {
            ItemDisplayLight disp = elementManager.ObjectAt(index).GetComponent<ItemDisplayLight>();
            disp.Init(slotInv);
            disp.gameObject.SetActive(true);
        }

        private void OnInventoryChanged(Inventory i, ItemSlot slot) {
            Validate();
        }
        
        /*
        public void TryTakeCursorItem() {
            if(!SingleItemSlot.TempSlot.Empty) {
                if(acceptsItems) {
                    TakeCursorItem();
                }
                //Debug.Log("Moved item: " + ItemOnCursor.item + " to inventory: " + this);
            }

        }

        public void TakeCursorItem() {
            if(AddItem(SingleItemSlot.TempSlot.Item)) {
                SingleItemSlot.TempSlot.RemoveItem();
            }

        }*/
    }
}