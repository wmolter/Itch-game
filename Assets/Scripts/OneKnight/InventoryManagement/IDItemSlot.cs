using UnityEngine;
using System.Collections;

namespace OneKnight.InventoryManagement {
    public class IDItemSlot : FilterItemSlot {

        string[] ids;

        public IDItemSlot(params string[] ids) : base((string)null) {
            this.ids = ids;
        }

        protected override bool Filter(InventoryItem toAdd) {
            for(int i = 0; i < ids.Length; i++) {
                if(ids[i] == toAdd.ID)
                    return true;
            }
            return false;
        }
    }
}