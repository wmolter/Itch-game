using UnityEngine;
using System.Collections;

namespace OneKnight.UI {
    public class FollowWorldObject : MonoBehaviour {

        public Camera reference;
        public Transform toFollow;
        public Vector2 offset;

        public Vector3 viewportPos { get; private set; }
        public bool inFront { get { return viewportPos.z > 0; } }
        public bool edgeWhenBehind = true;
        // Use this for initialization
        void OnEnable() {
            FollowObject();
        }

        void FollowObject() {

            viewportPos = reference.WorldToViewportPoint(toFollow.position);
            if(viewportPos.z < reference.nearClipPlane) {
                if(edgeWhenBehind) {
                    Vector3 temp = viewportPos - new Vector3(.5f, .5f, 0);
                    //take the closest to the center coordinate and multiply so that coordinate is past the edge of the viewport.
                    //Maintain the ratio of the other coordinate. This is designed to be used with a KeepOnScreen component.
                    temp *= -.5f/(Mathf.Max(Mathf.Abs(temp.x), Mathf.Abs(temp.y)));
                    viewportPos = temp + new Vector3(.5f, .5f, 0);
                }
            }
            RectTransform parent = (RectTransform)transform.parent;
            transform.localPosition = (Vector3)offset + new Vector3((viewportPos.x*2 - 1)*parent.rect.width/2, (viewportPos.y*2 - 1)*parent.rect.height/2, 0);
        }
        // Update is called once per frame
        void Update() {
            FollowObject();
        }
    }
}