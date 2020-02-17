using UnityEngine;
using System.Collections;

namespace OneKnight {
    [System.Serializable]
    public class Drop {
        public string id;
        public int count;

        public Drop(string id, int count) {
            this.id = id;
            this.count = count;
        }

        public override string ToString() {
            return (count > 1 ? count + " " : "") +  "<b>" + ItemInfo.Name(id) + "</b>";
        }
    }
}