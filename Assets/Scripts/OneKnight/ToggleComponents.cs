using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace OneKnight {
    public class ToggleComponents : MonoBehaviour {

        public List<MonoBehaviour> on;
        public List<MonoBehaviour> off;
        public bool startOn = true;
        public UnityEvent onToggle;
        bool currentState;

        public void Toggle() {
            currentState = !currentState;
            foreach(MonoBehaviour turnOn in on) {
                turnOn.enabled = currentState;
            }
            foreach(MonoBehaviour turnOff in off) {
                turnOff.enabled = !currentState;
            }
            onToggle?.Invoke();
        }
        // Use this for initialization
        void Awake() {
            currentState = !startOn;
        }

        private void OnEnable() {
            Toggle();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}