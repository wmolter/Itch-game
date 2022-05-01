using UnityEngine;
using System.Collections.Generic;
using OneKnight.Generation;

namespace OneKnight {
    [CreateAssetMenu(menuName ="Generation/Drop Manager")]
    public class DropManager : ScriptableObject {

        [System.Serializable]
        public struct Info {
            public DropTable table;
            public int rolls;
            //add one roll for every roll that is not empty
            public bool streak;
        }

        public DropManager() {
            guaranteed = new Drop();
        }
        public DropManager(List<InventoryItem> guaranteed) : this(new Drop(guaranteed)){
            
        }
        public DropManager(Drop guaranteed) {
            this.guaranteed = guaranteed;
        }

        [SerializeField]
        private Drop guaranteed;
        [SerializeField]
        private List<Info> rollFrom;

        public bool Rolled { get; private set; }
        //these methods erase the random amounts in the original drop
        /*
        public void AddGuaranteedItem(InventoryItem i) {
            List<InventoryItem> items = new List<InventoryItem>();
            items.AddRange(guaranteed.Generate());
            items.Add(i);
            guaranteed = new Drop(items);
        }

        public void AddGuaranteedItems(IEnumerable<InventoryItem> newItems) {
            List<InventoryItem> items = new List<InventoryItem>(newItems);
            items.AddRange(guaranteed.Generate());
            guaranteed = new Drop(items);
        } */


       public void Reset() {
            Rolled = false;
        }

        public IEnumerable<InventoryItem> RollDrops() {
            Rolled = true;
            if(guaranteed != null && guaranteed.Weight > 0) {
                foreach(InventoryItem i in guaranteed.Generate()) {
                    yield return i;
                }
            }
            foreach(Info data in rollFrom) {
                foreach(InventoryItem i in RollTable(data)) {
                    yield return i;
                }
            }
        }

        private IEnumerable<InventoryItem> RollTable(Info table) {
            if(table.streak) {
                int rolls = table.rolls;
                while(rolls > 0) {
                    int itemcount = 0;
                    foreach(InventoryItem i in table.table.Generate()) {
                        itemcount++;
                        yield return i;
                    }
                    //only decrease if you got nothing from the table, since we're streaking
                    if(itemcount == 0)
                        rolls--;
                }
            } else {
                foreach(InventoryItem i in table.table.Generate(table.rolls)) {
                    if(i == null)
                        continue;

                    yield return i;
                }
            }
        }
    }
}