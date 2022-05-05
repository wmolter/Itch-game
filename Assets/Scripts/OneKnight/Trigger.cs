using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace OneKnight {
    public class Trigger : MonoBehaviour {

        public UnityEvent onEnable;
        public UnityEvent onDisable;

        private void OnEnable() {
            onEnable.Invoke();
        }

        private void OnDisable() {
            onDisable.Invoke();
        }
    }

}