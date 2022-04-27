using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    [RequireComponent(typeof(NonPlayerEntity))]
    public class Enemy : MonoBehaviour {

        public BehaviorTree behavior;
        public List<string> enemyLayers;

        public Entity behaviorTarget;
        [HideInInspector]
        public Vector3 Home {
            get {
                return GetComponent<Entity>().home;
            }
        }
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void FixedUpdate() {
        }
    }
}