using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [System.Serializable]
    public struct Stun : IAttackEffect {
        public float duration;
        public void Attack(Health opponent, Entity self) {
            Stunnable s = opponent.GetComponent<Stunnable>();
            if(s != null && s.enabled) {
                s.Stun(duration);
            }
        }
    }
}