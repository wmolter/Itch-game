using System;
namespace OneKnight.InventoryManagement {
    public interface ItemHolder {
        bool AddItem(InventoryItem item);
        void RemoveItem(InventoryItem item);
    }
}
