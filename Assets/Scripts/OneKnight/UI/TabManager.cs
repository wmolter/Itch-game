using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace OneKnight.UI {
    public class TabManager : MonoBehaviour {

        public int startingTab = 0;
        public TabIndicator[] tabIndicators;
        public GameObject[] objectsInTabs;
        public int[] separatorIndices;
        public event UnityAction<int> OnActivate;

        public int currentTab { get; private set; }
        public TabIndicator CurrentIndicator { get {
                if(currentTab >= 0 && tabIndicators != null && currentTab < tabIndicators.Length)
                    return tabIndicators[currentTab].isActiveAndEnabled ? tabIndicators[currentTab] : null;
                else
                    return null;
            } }

        //
        public void ActivateTab(int index) {
            Debug.Log("Activating tab " + index);
            startingTab = index;
            currentTab = index;
            for(int i = 0; i < objectsInTabs.Length; i++) {
                bool active = (!(index < 0) && (index == separatorIndices.Length || i < separatorIndices[index]) && (index == 0 || i >= separatorIndices[index-1]));
                objectsInTabs[i].SetActive(active);
            }
            for(int i = 0; i < tabIndicators.Length; i++) {
                tabIndicators[i].Indicate(index == i);
            }
            OnActivate?.Invoke(index);
        }

        
        // Use this for initialization
        void OnEnable() {
            Validate();
            StartCoroutine(Utils.WaitOneFrame(ActivateTab, startingTab));
        }

        public void Validate() {
            int index = 0;
            foreach(TabIndicator indicator in tabIndicators) {
                indicator.Init(this, index);
                index++;
            }
        }
        

        // Update is called once per frame
        void Update() {

        }
    }
}