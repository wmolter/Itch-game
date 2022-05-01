using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using OneKnight.PropertyManagement;

namespace OneKnight.InventoryManagement {
    [System.Serializable]
    public abstract class ItemContainer : PropertyManaged, IEnumerable<ItemSlot>, IEnumerable<InventoryItem> {
        public abstract int Count { get; protected set; }

        //whether the slots have any *items*, not whether there are no slots
        public bool Empty { get { return Count == 0; } }

        public abstract IEnumerator<ItemSlot> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator<InventoryItem> IEnumerable<InventoryItem>.GetEnumerator() {
            foreach(ItemSlot slot in this) {
                if(!slot.Empty)
                    yield return slot.Item;
            }
        }

        public bool FunctionAvailable(string function) {
            foreach(ItemSlot slot in this) {
                if(!slot.Empty) {
                    if(slot.Item.HasProperty("functions")) {
                        List<string> funcs = new List<string>(slot.Item.GetBaseProperty<string[]>("functions"));
                        if(funcs.Contains(function)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public List<string> AvailableFunctions {
            get {
                List<string> allFunctions = new List<string>();
                foreach(ItemSlot slot in this) {
                    if(!slot.Empty) {
                        if(slot.Item.HasProperty("functions")) {
                            allFunctions.AddRange(slot.Item.GetBaseProperty<string[]>("functions"));
                        }
                    }
                }
                return allFunctions;
            }
        }

        public virtual InventoryItem ItemWithFunction(string function) {
            return ItemWithFunction(function, false);
        }

        public virtual InventoryItem ItemWithFunction(string function, bool prioConsumable) {
            InventoryItem temp = null;
            foreach(ItemSlot slot in this) {
                if(!slot.Empty) {
                    if(slot.Item.HasProperty("functions")) {
                        List<string> funcs = new List<string>(slot.Item.GetBaseProperty<string[]>("functions"));
                        if(funcs.Contains(function)) {
                            if(slot.Item.Consumable && prioConsumable) {
                                return slot.Item;
                            } else if(slot.Item.Consumable) {
                                temp = slot.Item;
                            } else if(prioConsumable) {
                                temp = slot.Item;
                            } else {
                                return slot.Item;
                            }
                        }
                    }
                }
            }
            return temp;
        }
    }
}