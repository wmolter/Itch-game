using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Entity))]
    public class NonPlayerEntity : MonoBehaviour {

        private void Awake() {
        }
        // Use this for initialization
        void Start() {
            CheckChangeVisibility();
        }

        // Update is called once per frame
        void Update() {
            CheckChangeVisibility();
        }

        void CheckChangeVisibility() {
            float sqrPlayerDist = Vector2.SqrMagnitude(transform.position - PlayerManager.instance.transform.position);
            GetComponent<Renderer>().enabled = PlayerManager.instance.SqrSightRange >= sqrPlayerDist;
        }
    }
}