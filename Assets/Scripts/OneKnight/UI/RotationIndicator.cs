using UnityEngine;
using System.Collections;

namespace OneKnight.UI {
    public class RotationIndicator : MonoBehaviour {


        public Utils.FloatGetter toDisplay;

        public float minRotation;
        public float maxRotation;

        private void Start() {
            SetRotation(maxRotation);
        }

        private void LateUpdate() {
            SetRotation(toDisplay());
        }

        void SetRotation(float percent) {
            transform.rotation = Quaternion.AngleAxis((maxRotation - minRotation)*percent + minRotation, Vector3.forward);
        }
    }
}