using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : MonoBehaviour {

        private List<Interaction> interactions;
        public Interaction Interaction { get {
                if(interactions == null || interactions.Count == 0)
                    return null;
                int prio = int.MinValue;
                int index = -1;
                for(int i = 0; i < interactions.Count; i++) {
                    if(interactions[i].Priority > prio) {
                        index = i;
                        prio = interactions[i].Priority;
                    }
                }
                return interactions[index];
            } }

        public void AddInteraction(Interaction action) {
            if(interactions == null) {
                interactions = new List<Interaction>();
            }
            if(!interactions.Contains(action))
                interactions.Add(action);
        }

        public void RemoveInteraction(Interaction action) {
            interactions?.Remove(action);
        }

        public bool Interact(PlayerManager player) {
            bool? ans = Interaction?.Interact(player);
            if(!ans.HasValue)
                return false;
            return ans.Value;
        }
    }
}