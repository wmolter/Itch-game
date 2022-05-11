using UnityEngine;
using System.Collections;

namespace OneKnight.UI {

    public class Follow2D : MonoBehaviour {

        public Vector2 offset;
        public Transform follow;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void LateUpdate() {
            transform.position = (Vector2)follow.position + offset;
        }
    }
}