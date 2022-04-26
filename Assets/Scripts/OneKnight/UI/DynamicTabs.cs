using UnityEngine;
using System.Collections;

namespace OneKnight.UI {
    [RequireComponent(typeof(TabManager))]
    public class DynamicTabs : MonoBehaviour {

        private int prevChildCount;
        
        // Use this for initialization
        void Start() {
            Refresh();
        }

        void Refresh() {
            TabIndicator[] indicators = new TabIndicator[transform.childCount];
            int index = 0;
            foreach(Transform child in transform) {
                indicators[index] = child.GetComponent<TabIndicator>();
                index++;
            }
            GetComponent<TabManager>().tabIndicators = indicators;
            GetComponent<TabManager>().Validate();
            prevChildCount = transform.childCount;
        }

        // Update is called once per frame
        void Update() {
            if(transform.childCount != prevChildCount) {
                Refresh();
            }
        }
    }
}