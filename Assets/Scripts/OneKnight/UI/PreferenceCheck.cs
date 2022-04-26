using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    [RequireComponent(typeof(Toggle))]
    public class PreferenceCheck : MonoBehaviour{

        public string prefName;
        // Use this for initialization
        void Start() {
            if(Preferences.Has(prefName))
                GetComponent<Toggle>().isOn = Preferences.GetToggle(prefName);
            else
                Preferences.UpdateAdd(prefName, GetComponent<Toggle>().isOn);
            GetComponent<Toggle>().onValueChanged.AddListener(SavePref);
        }

        private void OnDestroy() {
            GetComponent<Toggle>().onValueChanged.RemoveListener(SavePref);
        }

        public void SavePref(bool enabled) {
            Preferences.UpdateAdd(prefName, enabled);
        }
        // Update is called once per frame
        void Update() {

        }
    }
}