using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    [System.Serializable]
    [CreateAssetMenu(menuName = "Generation/DropTable")]
    public class DropTable : ScriptableObject, ItemEmitter {
        [SerializeField]
        private float weight = 1;

        [SerializeField]
        private List<DropTable> tables;
        [SerializeField]
        private List<Drop> singles;
        public float emptyWeight = 0;

        private float totalWeight;
        private List<ItemEmitter> drops;
        public float Weight { get { return weight; } }

        public DropTable() {

        }
        public DropTable(float weight) {
            this.weight = weight;
        }

        void Init() {
            drops = new List<ItemEmitter>(singles.Count + tables.Count);
            drops.AddRange(singles);
            drops.AddRange(tables);
            totalWeight = UniformGeneration.TotalWeight(drops) + emptyWeight;
        }

        public IEnumerable<InventoryItem> Generate() {
            if(drops == null)
                Init();
            ItemEmitter chosen = (ItemEmitter)UniformGeneration.RandomWeighted(drops, totalWeight);
            return chosen.Generate();
        }

        public IEnumerable<InventoryItem> Generate(int times) {
            for(int i = 0; i < times; i++) {
                foreach(InventoryItem item in Generate())
                    yield return item;
            }
        }
    }
}