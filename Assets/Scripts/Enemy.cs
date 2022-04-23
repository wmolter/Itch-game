using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(NonPlayerEntity))]
    public class Enemy : MonoBehaviour {

        public BehaviorTree behavior;
        public string[] enemyLayers;
        [HideInInspector]
        public Vector3 home;
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