using System;
using UnityEngine.Events;
using UnityEngine;
using OneKnight.PropertyManagement;
namespace OneKnight {
    [Serializable]
    public class ItemSlot : PropertyManaged, Displayable, Spriteable {

        //public delegate void SlotAction(InventoryItem item);
        //public event SlotAction OnAdd;
        //public event SlotAction OnRemove;
        public struct EventInfo {
            public ItemSlot slot;
            public InventoryItem before;
            public InventoryItem after;
        }

        private InventoryItem item;

        public event UnityAction<EventInfo> OnChange;
        public event UnityAction<EventInfo> BeforeChange;
        private bool suppressEvents;

        public InventoryItem Item {
            get {
                return item;
            }
        }

        public bool Empty {
            get {
                return item == null;
            }
        }

        public float Volume {
            get {
                return Empty ? 0 : Item.Volume;
            }
        }

        public float Value {
            get {
                return Empty ? 0 : Item.Value;
            }
        }

        public int Count {
            get {
                return Empty ? 0 : Item.count;
            }
        }

        public ItemSlot() {

        }

        public ItemSlot(InventoryItem startWith, PropertyManager properties) {
            item = startWith;
            Properties = properties;
            if(item != null)
                item.Properties = Properties;
        }

        public virtual bool Swap(ItemSlot other) {
            Debug.Log("Swap initialized.");
            //FireBeforeChange();
            //suppressing events causes items to be lost when switching with volume inventories.. either implement fix or leave on
            //suppressEvents = true;
            InventoryItem thisTemp = RemoveItem();
            InventoryItem otherTemp = other.RemoveItem();
            //if both don't work, put items back.
            if(!(PutItem(otherTemp) && other.PutItem(thisTemp))) {
                Debug.Log("Swap failed.");
                ReplaceItem(thisTemp);
                other.ReplaceItem(otherTemp);
                //suppressEvents = false;
                return false;
            }
            //suppressEvents = false;
            //FireChangeEvent(thisTemp);
            return true;
        }

        public virtual bool PutItem(InventoryItem toAdd) {
            if(item == null) {
                FireBeforeChange();
                item = toAdd;
                if(item != null)
                    item.Properties = Properties;
                FireChangeEvent(null);
                return true;
            }
            return false;
        }

        public virtual InventoryItem RemoveItem() {
            FireBeforeChange();
            InventoryItem removed = item;
            if(removed != null)
                removed.Properties = null;
            item = null;
            FireChangeEvent(removed);
            //OnRemove(removed);
            return removed;
        }

        public virtual InventoryItem RemoveOne() {
            if(item.count <= 1) {
                return RemoveItem();
            } else {
                FireBeforeChange();
                InventoryItem prev = item;
                item = item.RemoveOne();
                if(item != null)
                    item.Properties = Properties;
                FireChangeEvent(prev);
                return new InventoryItem(item.ID, 1);
            }
        }

        public virtual InventoryItem Remove(int amount) {
            if(amount == 0)
                return null;
            if(item.count <= amount)
                return RemoveItem();
            else {
                FireBeforeChange();
                InventoryItem prev = item;
                item = item.Remove(amount);
                if(item != null)
                    item.Properties = Properties;
                FireChangeEvent(prev);
                return new InventoryItem(item.ID, amount);
            }
        }

        public virtual bool CanStack(InventoryItem other) {
            return Empty || Item.CanStack(other);
        }

        //returns the remains of the input stack
        public virtual InventoryItem Stack(InventoryItem other) {
            if(other == null)
                return other;
            return Stack(other, other.count);
        }

        public virtual InventoryItem Stack(InventoryItem other, int max) {
            if(other == null)
                return null;
            if(item == null) {
                if(other.count <= max && PutItem(other)) {
                    return null;
                } else if(PutItem(new InventoryItem(other.ID, max))) {
                    return new InventoryItem(other.ID, other.count-max);
                } else {
                    return other;
                }
            }
            InventoryItem prev = item;
            int oldcount = item.count;
            FireBeforeChange();
            item = item.Stack(other, max);
            if(item != null)
                item.Properties = Properties;
            FireChangeEvent(prev);
            int remaining = other.count - (item.count - oldcount);
            if(remaining == 0)
                return null;
            else
                return new InventoryItem(other.ID, remaining);
        }

        public virtual int StackOnto(ItemSlot other) {
            return StackOnto(other, other.item.count);
        }

        //transfer up to max items to the other slot.  returns the number of items moved.
        public virtual int StackOnto(ItemSlot other, int max) {
            if(max == 0)
                return 0;
            //Debug.Log("Stacking onto initialized.");
            /*if(other.Empty) {
                InventoryItem removeTemp = Remove(max);
                //Debug.Log("Category: " + removeTemp.Category);
                if(other.PutItem(removeTemp)){
                    return max;
                } else {
                    //Debug.Log("Stacking onto failed.");
                    Stack(removeTemp);
                    return 0;
                }
            } else*/
            if(other.CanStack(Item)) {
                FireBeforeChange();
                InventoryItem prev = item;
                int prevcount = Count;
                item = other.Stack(item, max);
                if(item != null)
                    item.Properties = Properties;
                FireChangeEvent(prev);
                return prevcount - Count;
            } else {
                return 0;
            }
        }

        public virtual InventoryItem SplitStack() {
            int newcount = Item.count-Item.count/2;
            InventoryItem newStack = new InventoryItem(Item.ID, newcount);
            FireBeforeChange();
            InventoryItem prev = item;
            if(Item.count/2 == 0) {
                item = null;
            } else {
                item = new InventoryItem(Item.ID, Item.count/2);
            }
            if(item != null)
                item.Properties = Properties;
            FireChangeEvent(prev);
            return newStack;
        }

        //returns the item that is displaced
        public virtual InventoryItem ReplaceItem(InventoryItem toAdd) {
            InventoryItem old = RemoveItem();
            if(PutItem(toAdd))
                return old;
            FireBeforeChange();
            InventoryItem prev = item;
            item = old;
            if(item != null)
                item.Properties = Properties;
            FireChangeEvent(prev);
            return toAdd;
        }

        public virtual void FireChangeEvent(InventoryItem prev) {
            if(!suppressEvents)
                OnChange?.Invoke(new EventInfo { slot=this, before=prev, after=item });
        }

        public virtual void FireBeforeChange() {
            if(!suppressEvents)
                BeforeChange?.Invoke(new EventInfo { slot=this, before=item, after=null });
        }

        public string DisplayString(int verbosity) {
            if(item != null)
                return item.DisplayString(verbosity);
            else
                return null;
        }

        public string SpriteName() {
            if(item != null)
                return item.SpriteName();
            else
                return null;
        }

        public override void SetProperties(PropertyManager properties) {
            Properties = properties;
            if(item != null) {
                item.Properties = properties;
            }
        }
    }
}
