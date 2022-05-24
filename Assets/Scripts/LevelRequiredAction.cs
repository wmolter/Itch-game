using UnityEngine;
using System.Collections;

namespace Itch {
    public abstract class LevelRequiredAction : MonoBehaviour, Interaction {
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public Data data;
        public class Data : ScriptableObject {
            public bool checkLevel;
            public string capabilityRequired = "Mining";
            public float levelRequired = 0;
            public int gatherXP = 0;
        }

        protected void Awake() {
            if(GetComponentInParent<Interactable>() == null)
                Debug.LogError("LevelRequired component has no interactable parent. It will do nothing.");
        }

        protected virtual void Start() {
            InitData(data);
        }

        private void OnEnable() {
            GetComponentInParent<Interactable>().AddInteraction(this);
        }

        private void OnDisable() {
            GetComponentInParent<Interactable>()?.RemoveInteraction(this);
        }

        public bool Interact(PlayerManager player) {
            if(!data.checkLevel || player.HasCapabilityLevel(data.capabilityRequired, data.levelRequired)) {
                return DoAction(player);
            } else {
                Notifications.CreateError(transform.position, data.capabilityRequired + " " + data.levelRequired + " Required");
                return false;
            }
        }

        public virtual void InitData(Data data) {
            this.data = data;
        }

        protected abstract bool DoAction(PlayerManager player);
    }
}