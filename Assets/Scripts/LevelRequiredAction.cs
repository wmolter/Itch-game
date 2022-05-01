using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public abstract class LevelRequiredAction : MonoBehaviour, Interaction {
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public bool checkLevel;
        public string capabilityRequired = "Mining";
        public float levelRequired = 0;
        public int gatherXP = 0;


        private void OnEnable() {
            GetComponent<Interactable>().AddInteraction(this);
        }

        private void OnDisable() {
            GetComponent<Interactable>().RemoveInteraction(this);
        }

        public bool Interact(PlayerManager player) {
            if(!checkLevel || player.HasCapabilityLevel(capabilityRequired, levelRequired)) {
                return DoAction(player);
            } else {
                Notifications.CreateError(transform.position, capabilityRequired + " " + levelRequired + " Required");
                return false;
            }
        }

        protected abstract bool DoAction(PlayerManager player);
    }
}