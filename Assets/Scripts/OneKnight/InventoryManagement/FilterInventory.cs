using System;
using OneKnight.PropertyManagement;
namespace OneKnight {
    [System.Serializable]
    public class FilterInventory : Inventory {

        protected string[] categories;
        public FilterInventory(string category) : this(category, 20){
        }

        public FilterInventory(string category, int capacity) : this(new string[]{ category}, capacity) {
        }

        public FilterInventory(string[] categories, int capacity) : this(categories, capacity, null) {
        }

        public FilterInventory(string[] categories, int capacity, PropertyManager properties) : base(false, capacity, properties) {
            this.categories = categories;
            ValidateCapacity();
        }

        protected override ItemSlot CreateSlot(){
            return new FilterItemSlot(categories);
        }
    }
}
