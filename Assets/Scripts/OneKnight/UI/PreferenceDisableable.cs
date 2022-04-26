using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.UI {
    public class PreferenceDisableable : MonoBehaviour {

        public string prefName;
        public bool flip;
        public List<GameObject> targets;
        
        // Use this for initialization
        void Start() {
            if(Preferences.Has(prefName)) {
                bool enable = !Preferences.GetToggle(prefName);
                foreach(GameObject target in targets) {
                    target.SetActive(enable ^ flip);
                }
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}