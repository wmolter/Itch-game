using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace OneKnight.UI {
    public class PlayerWindowManager : MonoBehaviour {

        public string closeAllInput = "Inventory";
        public Transform left;
        public Transform right;
        public Transform center;
        public Transform full;
        public ActivateableWindow[] windows;
        public GameObject[] deactivateOnOpen;
        public GameObject[] deactivateOnFull;
        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        private ActivateableWindow inLeft;
        private ActivateableWindow inRight;
        private ActivateableWindow inCenter;
        private ActivateableWindow previous;
        

        private bool overriding = false;
        // Use this for initialization

        private void Awake() {
            foreach(ActivateableWindow window in windows) {
                window.windowManager = this;
            }

        }
        void Start() {
        }
        
        // Update is called once per frame
        void Update() {
            if(Input.GetButtonDown(closeAllInput) && AnythingOpen()) {
                CloseAll();
            } else {
                foreach(ActivateableWindow window in windows) {
                    if(window.showInput != "" && Input.GetButtonDown(window.showInput)) {
                        window.Toggle();
                    }
                }
            }
        }

        public bool AnythingOpen() {
            return !(inCenter == null && inLeft == null && inRight == null);
        }

        public void Open(ActivateableWindow window) {
            if(overriding) {
                overriding = false;
                SetFullDependentsEnabled(true);
                inCenter.Exit();
            }
            ActivateableWindow temp = null;
            if(inCenter != null) {
                 temp = previous;
            }
            Center(window);
            if(temp != null)
                temp.Exit();
        }
        
        public void Open(ActivateableWindow window, bool centerIfCan) {
            Open(window);
        }
        /*
        public void Open(ActivateableWindow window, bool centerIfCan) {
            if(overriding) {
                overriding = false;
                SetFullDependentsEnabled(true);
                inCenter.Exit();
            }
            if(inCenter != null) {
                Left(inCenter);
                inCenter = null;
                inLeft.Refresh();
            }
            if(centerIfCan && inLeft == null && inRight == null) {
                Center(window);
            } else if(inLeft == null) {
                Left(window);
            } else if(inRight == null) {
                Right(window);
            } else {
                if(previous == inLeft) {
                    ActivateableWindow toExit = inRight;
                    Right(window);
                    toExit.Exit();
                } else if(previous == inRight) {
                    ActivateableWindow toExit = inLeft;
                    Left(window);
                    toExit.Exit();
                }
            }
        }*/

        private void Full(ActivateableWindow window) {
            inCenter = window;
            ShowAt(window, full);
            SetFullDependentsEnabled(false);
        }

        private void Center(ActivateableWindow window) {
            inCenter = window;
            ShowAt(window, center);
        }

        private void Left(ActivateableWindow window) {
            inLeft = window;
            ShowAt(window, left);
        }

        private void Right(ActivateableWindow window) {
            inRight = window;
            ShowAt(window, right);
        }

        private void ShowAt(ActivateableWindow window, Transform parent) {
            //Debug.Log("Set parent of " + window.gameObject.name + " to " + parent.gameObject.name);
            window.transform.SetParent(parent, false);
            previous = window;
            SetDependentsEnabled(false);
            Canvas.ForceUpdateCanvases();
        }

        private void SetDependentsEnabled(bool enabled) {
            foreach(GameObject toEnable in deactivateOnOpen) {
                toEnable.SetActive(enabled);
            }
            if(enabled)
                OnClose.Invoke();
            else
                OnOpen.Invoke();
        }

        private void SetFullDependentsEnabled(bool enabled) {
            foreach(GameObject toEnable in deactivateOnFull) {
                toEnable.SetActive(enabled);
            }
        }

        //will be called by Activateable window on CloseAll
        public void Close(ActivateableWindow window) {
            if(inLeft == window) {
                inLeft = null;
            }
            if(inRight == window) {
                inRight = null;
            }
            if(inCenter == window) {
                inCenter = null;
                if(overriding)
                    overriding = false;
            }
            CheckFull();
        }

        public void CloseAll() {
            if(inLeft != null)
                inLeft.Exit();
            if(inRight != null)
                inRight.Exit();
            if(inCenter != null)
                inCenter.Exit();
        }

        public void CheckFull() {
            if(inLeft != null && inRight != null)
                return;
            if(inLeft != null && inLeft.likesCenter) {
                Center(inLeft);
                inLeft = null;
                inCenter.Refresh();
            } else if(inRight != null && inRight.likesCenter) {
                Center(inRight);
                inRight = null;
                inCenter.Refresh();
            } else {
                SetDependentsEnabled(true);
            }

            if(!overriding) {
                SetFullDependentsEnabled(true);
            }

        }


        public void Override(ActivateableWindow window) {
            CloseAll();
            Full(window);
            overriding = true;
        }
    }
}