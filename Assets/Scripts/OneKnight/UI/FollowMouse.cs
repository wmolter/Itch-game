using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class FollowMouse : MonoBehaviour {

        public Vector3 offset;

        private void OnEnable() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            transform.position = offset + Input.mousePosition;
        }
        // Update is called once per frame
        void LateUpdate() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            transform.position = offset + Input.mousePosition;
            
        }
    }
}