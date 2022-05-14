using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    [System.Serializable]
    [CreateAssetMenu(menuName = "Generation/DropTable")]
    public class DropTable : ScriptableObject {
        [System.Serializable]
        public class Child : ItemEmitter {
            public DropTable table;
            public float weight;
            public float Weight { get { return weight; } }

            public IEnumerable<InventoryItem> Generate() {
                return table.Generate();
            }

            public IEnumerable<InventoryItem> Generate(int times) {
                return table.Generate(times);
            }

        }
        [SerializeField]
        private List<Child> tables;
        [SerializeField]
        private List<Drop> singles;
        public float emptyWeight = 0;

        private float totalWeight;
        private List<ItemEmitter> drops;

        public DropTable() {

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
            if(chosen == null)
                yield break;
            foreach(InventoryItem i in chosen.Generate())
                yield return i;
        }

        public IEnumerable<InventoryItem> Generate(int times) {
            for(int i = 0; i < times; i++) {
                foreach(InventoryItem item in Generate())
                    yield return item;
            }
        }
    }
}