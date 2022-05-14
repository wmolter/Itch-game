using UnityEngine;
using System.Collections.Generic;
using Itch.Behavior;
using UnityEngine.Events;

namespace Itch {
    [RequireComponent(typeof(NonPlayerEntity), typeof(BehaviorTree))]
    public class Enemy : MonoBehaviour {
        
        public List<string> enemyLayers;

        public Entity behaviorTarget;

        public UnityEvent OnDeath;
        [HideInInspector]
        public Vector3 Home {
            get {
                return GetComponent<Entity>().home;
            }
        }

        public void Die() {
            OnDeath.Invoke();
        }

        public bool TargetGone() {
            return behaviorTarget == null || !behaviorTarget.gameObject.activeSelf;
        }

        public bool TargetDeadOrGone() {
            return TargetGone() || TargetDeadButExists();
        }

        public bool TargetDeadButExists() {
            if(TargetGone())
                return false;
            return !Health.Living(behaviorTarget);
        }

        public void AddEnemyLayer(string layer) {
            if(!enemyLayers.Contains(layer))
                enemyLayers.Add(layer);
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