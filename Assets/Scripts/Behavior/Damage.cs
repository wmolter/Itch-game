using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [System.Serializable]
    public class Damage : IAttackEffect {
        public int count = 1;
        public float amount;
        public bool ignoreArmor;
        public void Attack(Health opponent, Entity self) {
            for(int i = 0; i < count; i++)
                opponent.SafeChange(-amount, self, ignoreArmor);
        }
    }
}