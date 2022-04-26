using System.Collections.Generic;
using UnityEngine;
namespace OneKnight {
    [System.Serializable]
    public class FilterItemSlot : ItemSlot {
        List<string> categories;

        public FilterItemSlot(string category) {
            this.categories = new List<string>();
            categories.Add(category);
        }

        public FilterItemSlot(string[] categories) {
            if(categories != null) {
                this.categories = new List<string>();
                this.categories.AddRange(categories);
            }
        }

        public override bool PutItem(InventoryItem toAdd) {
            if(toAdd == null || Filter(toAdd))
                return base.PutItem(toAdd);
            return false;
        }

        protected virtual bool Filter(InventoryItem toAdd) {
            if(categories == null || categories.Count == 0)
                return true;
            return categories.Contains(toAdd.Category);
        }
    }
}
