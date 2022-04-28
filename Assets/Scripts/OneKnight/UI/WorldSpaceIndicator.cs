using UnityEngine;
using System.Collections;
using OneKnight.Cameras;

namespace OneKnight.UI {
    [ExecuteInEditMode]
    public class WorldSpaceIndicator : MonoBehaviour {
        public CameraController mainCamera;
        public Transform toScale;
        public float scaleFactor = .05f;
        // Use this for initialization
        protected void Start() {
        }

        // Update is called once per frame
        void Update() {
            toScale.localScale = Vector3.one * scaleFactor * mainCamera.ZoomFactor / transform.parent.lossyScale.x;
            toScale.rotation = Quaternion.LookRotation(mainCamera.transform.position - toScale.position, mainCamera.transform.up);
        }

    }
}