using UnityEngine;
using System.Collections.Generic;
using Itch.Behavior;

namespace Itch {
    [RequireComponent(typeof(NonPlayerEntity), typeof(BehaviorTree))]
    public class Enemy : MonoBehaviour {
        
        public List<string> enemyLayers;

        public Entity behaviorTarget;
        [HideInInspector]
        public Vector3 Home {
            get {
                return GetComponent<Entity>().home;
            }
        }

        public bool TargetGone() {
            return behaviorTarget == null || !behaviorTarget.gameObject.activeSelf;
        }

        public bool TargetDeadOrGone() {
            if(TargetGone())
                return true;
            return !Health.Living(behaviorTarget);
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