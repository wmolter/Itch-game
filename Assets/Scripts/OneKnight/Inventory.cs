using UnityEngine;
using System.Collections.Generic;
using OneKnight.PropertyManagement;
using OneKnight.InventoryManagement;

namespace OneKnight {
    [System.Serializable]
    public class Inventory : ItemContainer{
        
        //private event ItemSlot.SlotAction OnAdd;
        //private event ItemSlot.SlotAction OnRemove;

        public event InventoryEvent OnChange;
        public delegate void InventoryEvent(Inventory i, ItemSlot slot);
        List<ItemSlot> items;
        private int selected;
        public int SelectedIndex {
            get {
                return selected%items.Count;
            }
        }
        public InventoryItem Selected {
            get {
                return SelectedSlot.Item;
            }
        }

        public ItemSlot SelectedSlot {
            get {
                return items[selected%items.Count];
            }
        }

        private int capacity;
        public int Capacity {
            get {
                return capacity;
            }
            set {
                if(value >= 0) {
                    capacity = value;
                    ValidateCapacity();
                }  else
                    throw new UnityException("Inventory capacity cannot be negative: " + value);
            }
        }

        protected Inventory(bool init, int capacity) : this(init, capacity, null) { 
        }

        protected Inventory(bool init, int capacity, PropertyManager properties) {
            Properties = properties;
            this.capacity = capacity;
            items = new List<ItemSlot>();
            if(init)
                ValidateCapacity();
        }

        public Inventory() : this(20) {

        }

        public Inventory(int capacity) : this(true, capacity){
        }

        protected void ValidateCapacity() {
            for(int i = items.Count; i < capacity; i++) {
                AddSlot(CreateSlot());
            }
            while(items.Count > capacity) {
                items.RemoveAt(items.Count - 1);
            }
        }

        protected virtual ItemSlot CreateSlot() {
            return new ItemSlot();
        }

        protected void AddSlot(ItemSlot toAdd) {
            items.Add(toAdd);
            toAdd.Properties = Properties;
            toAdd.OnChange += OnItemChanged;
            if(Capacity < items.Count)
                capacity = items.Count;
        }

        protected virtual void OnItemChanged(ItemSlot changed) {
            OnChange(this, changed);
        }

        public List<InventoryItem> SetCapacity(int capacity) {
            for(int i = items.Count; i < capacity; i++) {
                AddSlot(CreateSlot());
            }
            List<InventoryItem> leftover = new List<InventoryItem>();
            while(items.Count > capacity) {
                if(!items[items.Count-1].Empty)
                    leftover.Add(items[items.Count-1].Item);
                items.RemoveAt(items.Count - 1);
            }
            return leftover;
        }

        public int FilledSlotCount() {
            int count = 0;
            foreach(ItemSlot slot in items) {
                if(slot.Item != null)
                    count++;
            }
            return count;
        }

        public float CalculateValue() {
            float totalValue = 0;
            foreach(ItemSlot slot in this) {
                totalValue += slot.Value;
            }
            return totalValue;
        }

        public bool HasItem(InventoryItem item) {
            foreach(ItemSlot slot in items) {
                if(slot.Item == item)
                    return true;
            }
            return false;
        }

        public bool HasItemType(InventoryItem item) {
            return HasItemType(item.ID);
        }

        public bool HasItemType(string id) {
            foreach(ItemSlot slot in items) {
                if(slot.Item != null && slot.Item.ID == id)
                    return true;
            }
            return false;
        }

        public virtual bool AddItem(InventoryItem item) {
            ItemSlot addTo = FindEmpty();
            if(addTo != null) {
                bool result = addTo.PutItem(item);
                return result;
            }
            return false;
        }

        public virtual List<InventoryItem> AddAll(ICollection<InventoryItem> items) {
            List<InventoryItem> result = new List<InventoryItem>();
            foreach(InventoryItem item in items) {
                if(!AddItem(item))
                    result.Add(item);
            }
            return result;
        }

        public virtual InventoryItem AddStackItem(InventoryItem item) {
            while(item != null && item.count > 0) {
                ItemSlot addTo = FindSlotWith(item.ID, false);
                if(addTo == null)
                    addTo = FindEmpty();
                if(addTo == null)
                    return item;
                item = addTo.Stack(item);
            }
            return item;
        }

        public virtual List<InventoryItem> AddStackAll(ICollection<InventoryItem> items) {
            List<InventoryItem> result = new List<InventoryItem>();
            foreach(InventoryItem item in items) {
                InventoryItem temp = AddStackItem(item);
                if(temp != null)
                    result.Add(temp);
            }
            return result;
        }

        public virtual InventoryItem AddReplaceItem(InventoryItem item, int index) {
            if(index < 0 || index >= capacity)
                throw new UnityException("Trying to reference nonexistent Inventory slot: " + index);
            ItemSlot slot = items[index];
            bool initial = slot.Empty;
            InventoryItem toReturn = slot.ReplaceItem(item);
            bool final = slot.Empty;
            return toReturn;
        }

        protected virtual ItemSlot FindEmpty() {
            for(int i = 0; i < items.Count; i++) {
                if(items[i].Empty)
                    return items[i];
            }
            return null;
        }

        protected virtual ItemSlot FindSlotWith(string itemId, bool allowFull) {
            for(int i = 0; i < Capacity; i++) {
                if(!GetSlot(i).Empty && Get(i).ID == itemId && (allowFull || !Get(i).Full))
                    return GetSlot(i);
            }
            return null;
        }

        public int CountOf(string itemId) {
            int total = 0;
            for(int i = 0; i < Capacity; i++) {
                if(!GetSlot(i).Empty && Get(i).ID == itemId) {
                    total += GetSlot(i).Item.count;
                }
            }
            return total;
        }

        public virtual InventoryItem RemoveItem(int index) {
            InventoryItem item = items[index].RemoveItem();
            return item;
        }

        public virtual void RemoveItem(InventoryItem item) {
            for(int i = 0; i < items.Count; i++) {
                if(item == items[i].Item) {
                    items[i].RemoveItem();
                    return;
                }
            }
            throw new UnityException("Item not in inventory to be removed: " + item);
        }

        public virtual bool RemoveOne(string itemId) {
            for(int i = 0; i < items.Count; i++) {
                if(!items[i].Empty && itemId == items[i].Item.ID) {
                    items[i].RemoveOne();
                    return true;
                }
            }
            return false;
        }

        public virtual InventoryItem RemoveItem(string name) {
            for(int i = 0; i < items.Count; i++) {
                if(!items[i].Empty && items[i].Item.Name == name) {
                    InventoryItem item = items[i].RemoveItem();
                    return item;
                }
            }
            return null;
        }

        public virtual bool RemoveAmount(string itemId, int amount) {
            int total = 0;
            ItemSlot removeFrom = FindSlotWith(itemId, true);
            while(removeFrom != null && total < amount) {
                InventoryItem removed = removeFrom.Remove(amount-total);
                total += removed.count;
                removeFrom = FindSlotWith(itemId, true);
            }
            if(total == amount)
                return true;
            else {
                if(AddStackItem(new InventoryItem(itemId, total)) != null)
                    throw new UnityException("Failed to readd items after failed removal. ItemID: " + itemId + " total: " + total);
                return false;
            }
        }

        public virtual void Clear() {
            for(int i = 0; i < Capacity; i++) {
                GetSlot(i).RemoveItem();
            }
        }

        public void Sort(bool stack) {
            InventoryItem[] allItems = new InventoryItem[Capacity];
            for(int i = 0; i < Capacity; i++) {
                allItems[i] = GetSlot(i).Item;
            }
            Clear();
            System.Array.Sort(allItems);
            for(int i = 0; i < allItems.Length; i++) {
                if(allItems[i] == null)
                    continue;
                if(stack)
                    AddStackItem(allItems[i]);
                else
                    AddItem(allItems[i]);
            }
        }

        public void Compress(bool stack) {
            InventoryItem[] allItems = new InventoryItem[Capacity];
            for(int i = 0; i < Capacity; i++) {
                allItems[i] = GetSlot(i).Item;
            }
            Clear();
            for(int i = 0; i < allItems.Length; i++) {
                if(allItems[i] == null) {
                    continue;
                }
                if(stack)
                    AddStackItem(allItems[i]);
                else
                    AddItem(allItems[i]);
            }
        }


        public virtual void Select(int index) {
            if(index < 0)
                index += items.Count;
            selected = index % items.Count;
        }

        public InventoryItem Get(int index) {
            return items[index%items.Count].Item;
        }

        public ItemSlot GetSlot(int index) {
            return items[index%items.Count];
        }

        protected void RemoveSlotAt(int index) {
            //in case the slot is grabbed to use elsewhere with GetSlot
            items[index].Properties = null;
            items.RemoveAt(index);
            capacity--;
        }

        public override IEnumerator<ItemSlot> GetEnumerator() {
            for(int i = 0; i < Capacity; i++) {
                yield return GetSlot(i);
            }
        }

        public void CopyInto(Inventory other) {
            other.Clear();
            other.capacity = Capacity;
            other.ValidateCapacity();
            int index = 0;
            foreach(ItemSlot slot in this) {
                other.GetSlot(index).PutItem(slot.Item);
                index++;
            }
        }

        public override void SetProperties(PropertyManager properties) {
            Properties = properties;
            foreach(ItemSlot slot in this) {
                slot.SetProperties(properties);
            }
        }
    }
}