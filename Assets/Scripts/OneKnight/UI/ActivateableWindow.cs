using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace OneKnight.UI {
    public class ActivateableWindow : MonoBehaviour {

        [HideInInspector]
        public PlayerWindowManager windowManager;
        public GameObject displayContainer;
        public bool mustBeFull = false;
        public bool likesCenter = true;

        public UnityEvent onClose;

        public string showInput;
        public bool showing {
            get;
            protected set;
        }
        
        protected virtual void Start() {
            showing = false;
        }

        public void Toggle() {
            if(showing)
                Exit();
            else
                Open();
        }

        public void Show(bool visible) {
            if(displayContainer != null)
                displayContainer.SetActive(visible);
            showing = visible;
        }

        public void Refresh() {
            Show(false);
            Show(true);
        }

        public virtual void Open() {
            Show(true);
            //Debug.Log("Activateable open: " + gameObject.name);
            if(mustBeFull)
                windowManager.Override(this);
            else
                windowManager.Open(this, likesCenter);
        }

        public virtual void Exit() {
            windowManager.Close(this);
            Show(false);
            onClose.Invoke();
        }
    }
}