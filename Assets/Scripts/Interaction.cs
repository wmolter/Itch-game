using UnityEngine;
using System.Collections;

namespace Itch {
    public interface Interaction {
        int Priority { get; }

        bool Interact(PlayerManager player);
    }
}