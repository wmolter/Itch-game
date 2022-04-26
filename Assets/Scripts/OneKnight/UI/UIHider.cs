using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.UI {
    public class UIHider : MonoBehaviour {

        public GameObject[] toToggle;
        public List<GameObject> oppositeToggle;
        private GameObject[] indicators;
        public bool startEnabled = true;

        private void Start() {
            toggle = startEnabled;
            //SetUIEnabled(startEnabled);
        }

        private bool toggle;

        private void Update() {
            if(Input.GetButtonDown("HideUI")) {
                toggle = !toggle;
                SetUIEnabled(toggle);
            }
        }

        public void SetUIEnabled(bool enabled) {
            foreach(GameObject obj in toToggle) {
                obj.SetActive(enabled);
            }
            foreach(GameObject obj in oppositeToggle) {
                obj.SetActive(!enabled);
            }
            SetIndicators(enabled, true);
        }



        public void SetIndicators(bool enabled, bool force) {
            //force refresh only when trying to disappear, not on appear
            if(indicators == null || (force && !enabled)) {
                indicators = GameObject.FindGameObjectsWithTag(OKConstants.IndicatorTag);
            }

            foreach(GameObject helper in indicators) {
                helper.SetActive(enabled);
            }
        }
    }
}