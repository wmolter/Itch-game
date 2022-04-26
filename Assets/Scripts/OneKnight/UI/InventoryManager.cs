using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using OneKnight.InventoryManagement;

namespace OneKnight.UI {
    public class InventoryManager : MonoBehaviour, ItemHolder, IEnumerable<SingleItemSlot>, IPointerClickHandler, System.IComparable<InventoryManager> {

        public static List<InventoryManager> transfers;
        public static bool transfersSorted = false;

        public static void SortTransfers() {
            if(transfers == null || transfersSorted)
                return;
            transfers.Sort();
            transfersSorted = true;
        }

        public IEnumerable<ItemSlot> Inventory { get; private set; }
        public ReusableElementManager elementManager;
        public List<SingleItemSlot> customSlots;

        public bool ValidateOnChildChange;
        public List<string> categories;

        public int SlotCount { get; private set; }
        public int transferPriority = 0;
        public bool acceptsItems = true;
        public bool canTakeItems = true;
        public bool transferEligible = true;
        public bool tradeMode = false;
        public TradeSlot valueDeterminer;

        // Use this for initialization

        public void SetInventory(Inventory toDisplay) {
            SetInventory((IEnumerable<ItemSlot>)toDisplay);
        }

        public void SetInventory(IEnumerable<ItemSlot> toDisplay) {
            Inventory = toDisplay;
            Validate();
        }

        public void SetAccessible(bool acceptsItems, bool canTakeItems) {
            this.acceptsItems = acceptsItems;
            this.canTakeItems = canTakeItems;
            if(gameObject.activeInHierarchy) {
                Validate();
            }
        }

        public void Validate() {
            if(Inventory != null) {
                //Debug.Log("Called parent validate, and inventory slots are: " + Inventory.Capacity);
                int index = 0;
                foreach(ItemSlot slotInv in Inventory) {
                    if(Filter(slotInv)) {
                        InitSlot(slotInv, index);
                        index++;
                    }
                }
                SlotCount = index;
                //Debug.Log("Disabled from: " + index);
                elementManager.DisableFrom(Mathf.Max(0, index - customSlots.Count));
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
            if(!result) {
                foreach(string cat in categories) {
                    result |= slot.Item.HasProperty(cat);
                }
            }
            return result;
        }

        protected virtual void InitSlot(ItemSlot slotInv, int index) {
            SingleItemSlot slot;
            if(index < customSlots.Count) {
                slot = customSlots[index];
            } else {
                slot = elementManager.ObjectAt(index-customSlots.Count).GetComponent<SingleItemSlot>();
            }
            if(!slot.overrideParent) {
                slot.acceptsItems = acceptsItems;
                slot.canTakeItems = canTakeItems;
                slot.tradeMode = tradeMode;
            }
            slot.manager = this;
            slot.ToDisplay = slotInv;
            slot.validateParent = ValidateOnChildChange;
            slot.gameObject.SetActive(true);
        }

        public IEnumerator<SingleItemSlot> GetEnumerator() {
            foreach(SingleItemSlot slot in customSlots) {
                yield return slot;
            }
            foreach(GameObject disp in elementManager) {
                yield return disp.GetComponent<SingleItemSlot>();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            yield return GetEnumerator();
        }

        public bool AddItem(InventoryItem item) {
            if(item == null)
                return false;
            foreach(SingleItemSlot slot in this) {
                if(slot.AddItem(item)) {
                    Validate();
                    return true;
                }
            }
            return false;
        }


        public int Stack(ItemSlot otherslot, int max) {
            if(otherslot.Empty)
                return 0;
            int amountStacked = 0;
            foreach(SingleItemSlot slotui in this) {
                    //this will stack onto every slot in the inventory while the item lasts, but prioritize already existing stacks
                ItemSlot slot = (ItemSlot)slotui.ToDisplay;
                if(!slot.Empty && !otherslot.Empty && slot.Item.ID == otherslot.Item.ID) {
                    amountStacked += slotui.TryStackOrAdd(otherslot, max - amountStacked);
                }
            }
            if(!otherslot.Empty) {
                foreach(SingleItemSlot slotui in this) {
                    amountStacked += slotui.TryStackOrAdd(otherslot, max - amountStacked);
                }
            }
            Validate();
            return amountStacked;
         
        }


        public virtual void RemoveItem(InventoryItem item) {
            foreach(ItemSlot slot in Inventory) {
                if(item == slot.Item) {
                    slot.RemoveItem();
                    return;
                }
            }
            Validate();
        }

        public float SellValue(InventoryItem item) {
            return valueDeterminer.SellValuePer(item);
        }

        protected virtual void OnEnable() {
            //Debug.Log("Inventory enable.");
            Validate();
            if(!transferEligible)
                return;
            if(transfers == null)
                transfers = new List<InventoryManager>();
            transfers.Add(this);
            transfersSorted = false;
        }

        protected virtual void OnDisable() {
            if(transferEligible)
                transfers.Remove(this);
        }


        public void OnPointerClick(PointerEventData data) {
            /*
            Debug.Log("Click on inventory recognized.");
            TryTakeCursorItem();
            SingleItemSlot.UpdateTooltip();*/
        }

        public int CompareTo(InventoryManager other) {
            return transferPriority - other.transferPriority;
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