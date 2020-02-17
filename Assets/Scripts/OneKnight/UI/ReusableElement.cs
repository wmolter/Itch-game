using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace OneKnight.UI {
    public class ReusableElement : MonoBehaviour {

        private ReusableElementManager manager;

        private void OnDisable() {
            //manager will be null when IT gets disabled; in that case, these should NOT be added to its available list, as they will be reappearing when it is enabled
            if(manager != null)
                manager.OnDisabled(this);
        }

        public void Init(ReusableElementManager manager) {
            this.manager = manager;
        }
    }
}