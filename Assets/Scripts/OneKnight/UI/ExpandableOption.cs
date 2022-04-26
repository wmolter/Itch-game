using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace OneKnight.UI {
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public class ExpandableOption : MonoBehaviour {

        private bool state;
        public event UnityAction OnPreExpand;
        public event UnityAction OnAction;
        // Use this for initialization
        void Start() {
            SetExpandedState(state);
        }

        // Update is called once per frame
        void Update() {

        }

        public void Expand() {
            SetExpandedState(true);
            OnAction();
        }

        public void Contract() {
            SetExpandedState(false);
            if(OnAction != null)
                OnAction();
        }

        public void Toggle() {
            SetExpandedState(!state);
            if(OnAction != null)
                OnAction();
        }

        public void SetExpandedState(bool expanded) {
            state = expanded;
            if(expanded && OnPreExpand != null)
                OnPreExpand();
            Debug.Log("Changed expanded state.");
            foreach(Transform child in transform) {
                child.gameObject.SetActive(expanded);
            }
        }
    }
}