using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : MonoBehaviour {

        public delegate bool InteractionAction(PlayerManager player);

        public InteractionAction Interaction;

        public bool Interact(PlayerManager player) {
            bool? ans = Interaction?.Invoke(player);
            if(!ans.HasValue)
                return false;
            return ans.Value;
        }
    }
}