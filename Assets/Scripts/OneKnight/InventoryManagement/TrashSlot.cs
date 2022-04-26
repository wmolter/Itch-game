using UnityEngine;
using System.Collections;

namespace OneKnight.InventoryManagement {
    public class TrashSlot : FilterItemSlot {

        public TrashSlot() : base((string[])null) {

        }

        public TrashSlot(string category) : base(category) {

        }

        public override bool PutItem(InventoryItem toAdd) {
            RemoveItem();
            return base.PutItem(toAdd);
        }

        public override bool CanStack(InventoryItem other) {
            return true;
        }

        public override InventoryItem Stack(InventoryItem other, int max) {
            RemoveItem();
            return base.Stack(other, max);
        }
    }
}