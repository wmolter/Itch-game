using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    public interface ItemEmitter:Weighted {

        IEnumerable<InventoryItem> Generate();
        IEnumerable<InventoryItem> Generate(int times);
    }
}