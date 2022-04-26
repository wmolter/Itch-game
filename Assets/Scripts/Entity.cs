using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Entity : MonoBehaviour {

        public float sortingOffset;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void LateUpdate() {
            GetComponent<SpriteRenderer>().sortingOrder = (int)(-32*(transform.position.y+sortingOffset));
        }
    }
}