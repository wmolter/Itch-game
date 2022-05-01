using UnityEngine;
using System;
using System.Collections.Generic;
using OneKnight.Loading;
using OneKnight.PropertyManagement;

namespace OneKnight {
    [Serializable]
    public class InventoryItem : PropertyManaged, Displayable, Spriteable, IComparable<InventoryItem> {
        public static void CompressList(List<InventoryItem> items, bool stackSafe) {
            int index = items.Count - 1;
            while(index > 1) {
                InventoryItem current = items[index];
                for(int i = 0; i < index; i++) {
                    if(current.ID == items[i].ID) {
                        current = items[i].Stack(current, stackSafe);
                        int change = current.count - items[i].count;
                        items[i] = current;
                        current = new InventoryItem(current.ID, items[index].count - change, stackSafe);
                        items[index] = current;
                        if(items[index].Count == 0)
                            break;
                    }
                }
                if(items[index].Count == 0)
                    items.RemoveAt(index);
                index--;
            }
        }
        public class Data : Loading.FullDescription, ArgumentHolder {

            public int sortingID;
            //inward facing
            public string category = "";
            public string spriteName = "";
            public float baseValue;
            public int stackLimit;
            public float volumePer;

            public Dictionary<string, object> args { get; set; }
            public string[] argOrder { get; set; }

            public virtual InventoryItem Create() {
                return Create(1);
            }

            public virtual InventoryItem Create(int count) {
                return Create(count, true);
            }

            public virtual InventoryItem Create(int count, bool stackSafe) {
                return new InventoryItem(id, count, stackSafe);
            }

            //use args to put things in their places, if you wish
            public virtual void AssignProperties() {

            }

            protected T Assign<T>(string property, T dfault) {
                if(args.ContainsKey(property))
                    return (T)args[property];
                return dfault;
            }

            protected void Require<T>(string property, out T fill) {
                if(args.ContainsKey(property))
                    fill = (T)args[property];
                else
                    throw new UnityException("Items of type " + category + " are required to have a " + property + " property.");
            }

        }

        [SerializeField]
        private string id;
        public string ID {
            get {
                return id;
            }
        }

        public int SortingID {
            get {
                return ItemInfo.SortingID(id);
            }
        }

        public float ValueEach {
            get {
                return ItemInfo.BaseValue(id);
            }
        }

        public float Value {
            get {
                return ValueEach*count;
            }
        }

        public string Name {
            get {
                return ItemInfo.Name(id);
            }
        }
        public string Description {
            get {
                return ItemInfo.Description(id);
            }
        }

        public string FlavorText {
            get {
                return ItemInfo.Flavor(ID);
            }
        }

        protected bool Stackable {
            get {
                return StackLimit > 1;
            }
        }


        [SerializeField]
        private int Count;

        public int count {
            get { return Count; }
            private set { Count = value; }
        }

        public float Volume {
            get {
                return count * VolumePer;
            }
        }

        public float VolumePer {
            get {
                return ItemInfo.VolumePer(id);
            }
        }

        public int StackLimit {
            get {
                return ItemInfo.StackLimit(id);
            }
        }

        public bool Full {
            get {
                return count == StackLimit;
            }
        }

        public bool Consumable {
            get {
                return Category == ItemInfo.Category.Consumable;
            }
        }

        public string Category {
            get {
                return ItemInfo.GetCategory(id);
            }
        }

        public T GetBaseProperty<T>(string propertyName) {
            return ItemInfo.GetProperty<T>(id, propertyName);
        }

        public float GetFloatProperty(string propertyName) {
            float prop = GetBaseProperty<float>(propertyName);
            return AdjustProperty(propertyName, prop);
        }

        public bool HasProperty(string propertyName) {
            return ItemInfo.HasProperty(id, propertyName);
        }

        public virtual System.Type ControllerType {
            get {
                return null;
            }
        }

        protected InventoryItem() {
            //for serialization
        }

        public InventoryItem(InventoryItem drop) : this(drop.id, drop.count) {

        }

        public InventoryItem(string id, int count) : this(id, count, true){

        }

        public InventoryItem(string id, int count, bool stackSafe) {
            this.id = id;
            if(stackSafe && StackLimit != 0 && count > StackLimit)
                throw new UnityException("Tried to create a " + Name + " with too many stacks.  Stack size: " + count + " Limit: " + StackLimit);
                
            this.count = count;
        }

        public virtual InventoryItem Copy() {
            return new InventoryItem(id, count);
        }

        public InventoryItem(string id) : this(id, 1) {
        }

        public InventoryItem RemoveOne() {
            if(count == 0)
                throw new UnityException("Tried to remove an item with 0 stacks.");
            return new InventoryItem(id, count-1);
        }

        public InventoryItem Remove(int amount) {
            Debug.Log("Item remove called.");
            if(amount == 0)
                return this;
            int newcount = Mathf.Max(0, count-amount);
            if(newcount == 0)
                return null;
            return new InventoryItem(id, newcount);
        }

        public bool CanStack(InventoryItem other) {
            return other.ID == ID && !Full;
        }

        //returns the new inventoryitem
        public InventoryItem Stack(InventoryItem other) {
            return Stack(other, other.count);
        }

        public InventoryItem Stack(InventoryItem other, bool stackSafe) {
            return Stack(other, other.count, stackSafe);
        }

        public InventoryItem Stack(InventoryItem other, int max) {
            return Stack(other, max, true);
        }


        public InventoryItem Stack(InventoryItem other, int max, bool stackSafe) {
            if(other.ID != ID)
                throw new UnityException("Tried to stack two different items: " + id + " and " + other.id);
            int newcount = count + Mathf.Min(max, other.count);
            if(stackSafe)
                newcount = Mathf.Min(StackLimit, newcount);
            return new InventoryItem(ID, newcount, stackSafe);
        }


        public virtual string DisplayString(int verbosity) {
            string result = (count > 1 ? count + " ": "") +  "<b>" + Name + "</b>";
            if(verbosity >= 3) {
                result += "\n" + Description;
            }
            if(verbosity >= 5) {
                result += "\n" + FlavorText;
            }
            return result;
        }

        public override string ToString() {
            return DisplayString(1);
        }

        public int CompareTo(InventoryItem other) {
            if(other == null)
                return -1;
            if(other.SortingID == SortingID)
                return other.count - count;
            return SortingID - other.SortingID;
        }

        public string SpriteName() {
            return ItemInfo.SpriteName(ID);
        }

        public static bool operator ==(InventoryItem a, InventoryItem b) {
            return (ReferenceEquals(a, null) && ReferenceEquals(b, null)) || (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.ID == b.ID && a.count == b.count);
        }

        public static bool operator !=(InventoryItem a, InventoryItem b) {
            return !(ReferenceEquals(a, null) && ReferenceEquals(b, null)) && (ReferenceEquals(a, null) && !ReferenceEquals(b, null) || !ReferenceEquals(a, null) && ReferenceEquals(b, null) || a.count != b.count || a.ID != b.ID);
        }
    }
}