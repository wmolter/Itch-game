using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public abstract class LevelRequiredAction : MonoBehaviour {
        public bool checkLevel;
        public string capabilityRequired = "Mining";
        public float levelRequired = 0;


        private void OnEnable() {
            GetComponent<Interactable>().Interaction = CheckDo;
        }

        public void CheckDo(PlayerManager player) {
            if(!checkLevel || player.HasCapabilityLevel(capabilityRequired, levelRequired)) {
                DoAction(player);
            } else {
                Notifications.CreateError(transform.position, capabilityRequired + " " + levelRequired + " Required");
            }
        }

        protected virtual void DoAction(PlayerManager player) {

        }
    }
}