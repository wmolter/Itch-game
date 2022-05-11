using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Enterable : MonoBehaviour, Interaction {

        public int destination;
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public Enterable exitPrefab;
        // Use this for initialization

        private void OnEnable() {
            GetComponent<Interactable>().AddInteraction(this);
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

        }

        public bool Interact(PlayerManager player) {
            Planes.Activate(destination);
            if(exitPrefab != null) {
                Enterable newObj = Instantiate<Enterable>(exitPrefab, Planes.CurrentPlane.map.transform);
                newObj.transform.position = transform.position;
            }
            player.GetComponent<FollowerManager>().FormUp();
            return true;
        }
    }
}