using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    [System.Serializable]
    public class Drop : ItemEmitter{
        [SerializeField]
        private string[] ids;
        [SerializeField]
        private Vector2Int[] quantities;
        public float weight = 1;

        public float Weight { get { return weight; } }


        public Drop() {

        }

        public Drop(string[] id, Vector2Int[] quantities, float weight){
            this.ids = id;
            this.weight = weight;
            if(quantities.Length != ids.Length) {
                throw new System.Exception("Arrays must be the same length.");
            }
            this.quantities = quantities;
        }

        public IEnumerable<InventoryItem> Generate() {
            for(int i = 0; i < ids.Length; i++) {
                int count = Random.Range(quantities[i].x, quantities[i].y+1);
                yield return new InventoryItem(ids[i], count);
            }
        }

        public IEnumerable<InventoryItem> Generate(int times) {
            for(int i = 0; i < times; i++) {
                foreach(InventoryItem item in Generate())
                    yield return item;
            }
        }
    }
}