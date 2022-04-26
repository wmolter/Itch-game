using UnityEngine;
using System.Collections;

namespace OneKnight.UI {
    public abstract class ObjectDisplay : MonoBehaviour {

        private object toDisplay;
        public object ToDisplay {
            get {
                return toDisplay;
            }
            set {
                toDisplay = value;
                Validate();
            }
        }
        public bool refreshOnUpdate = false;

        protected virtual void Start() {
            if(ToDisplay != null)
                Validate();
        }

        protected virtual void Update() {
            if(refreshOnUpdate)
                Validate();
        }

        public abstract void Validate();

    }
}