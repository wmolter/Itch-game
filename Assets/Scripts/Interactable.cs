using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : MonoBehaviour {

        public delegate void InteractionAction(PlayerManager player);

        public InteractionAction Interaction;

        public void Interact(PlayerManager player) {
            Interaction?.Invoke(player);
        }
    }
}