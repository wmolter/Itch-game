using UnityEngine;
using System.Collections;
using Itch.Behavior;

namespace Itch {
    [RequireComponent(typeof(Interactable), typeof(Enemy))]
    public class Tamed : MonoBehaviour, Interaction {
        
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public int handlingCost = 3;
        public bool follow = false;
        public string newLayer = "Player";
        public FollowerManager manager;
        public BehaviorNode followBehavior;
        private BehaviorNode prevBehavior;
        private Vector2 prevHome;

        private bool previousEnemy;
        int prevLayer;
        // Use this for initialization

        private void OnEnable() {
            GetComponent<Interactable>().AddInteraction(this);
            BeTamed();
        }

        private void OnDisable() {
            GetComponent<Interactable>().RemoveInteraction(this);
        }
        private void Start() {
            if(enabled) {
                GetComponent<Interactable>().AddInteraction(this);
            }
        }

        // Update is called once per frame
        void Update() {
            if(follow) {
                GetComponent<Entity>().home = manager.transform.position;
            }
        }

        public void BeTamed() {
            //important single or here, want to remove regardless
            previousEnemy = previousEnemy | GetComponent<Enemy>().enemyLayers.Remove(newLayer);
            Notifications.CreateNotification(transform.position, "Tamed");
        }

        public void GoWild() {
            enabled = false;
            EndFollow();
            if(previousEnemy)
                GetComponent<Enemy>().AddEnemyLayer(newLayer);
            Notifications.CreateNotification(transform.position, "No longer tame");
        }

        public void OnDeath() {
            enabled = false;
            if(manager != null)
                manager.ToggleFollower(this);
        }

        public void StartFollow(FollowerManager manager) {
            this.manager = manager;
            prevLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer(newLayer);
            //this allows followers through portals.  I could create additional Enterable code..
            transform.SetParent(null, true);
            prevHome = GetComponent<Entity>().home;
            prevBehavior = GetComponent<BehaviorTree>().root;
            if(followBehavior != null && prevBehavior != followBehavior) {
                GetComponent<BehaviorTree>().SetRoot(followBehavior);
            }
            follow = true;
            Notifications.CreateNotification(transform.position, "Now following");
        }

        public void EndFollow() {
            manager = null;
            follow = false;
            gameObject.layer = prevLayer;
            transform.SetParent(Planes.CurrentPlane.transform, true);
            if(followBehavior != null && prevBehavior != followBehavior) {
                GetComponent<BehaviorTree>().SetRoot(prevBehavior);
            }
            if(Planes.CurrentPlaneIndex == GetComponent<Entity>().homePlane)
                GetComponent<Entity>().home = prevHome;
            else {
                prevHome = GetComponent<Entity>().home = transform.position;
            }
            // I would like to suppress this when dying... also catches when direct to wild, which is good
            if(enabled)
                Notifications.CreateNotification(transform.position, "No longer following");
        }

        public bool Interact(PlayerManager player) {
            return player.GetComponent<FollowerManager>().ToggleFollower(this);
        }
    }
}