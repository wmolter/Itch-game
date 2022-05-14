using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Itch.Behavior {
    public interface IAttackEffect {
        void Attack(Health opponent, Entity self);
    }

    [CreateAssetMenu(menuName ="Attack Effect")]
    public class AttackEffect : ScriptableObject, IEnumerable<IAttackEffect>, IAttackEffect {
        public List<Damage> damages;
        public List<PoisonHealing> overtimes;
        public Stun stun;

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<IAttackEffect> GetEnumerator() {
            foreach(Damage d in damages) {
                yield return d;
            }
            foreach(PoisonHealing dps in overtimes) {
                yield return dps;
            }
            if(stun.duration > 0)
                yield return stun;
        }

        public void Attack(Health opponent, Entity self) {
            foreach(IAttackEffect effect in this) {
                effect.Attack(opponent, self);
            }
        }
    }
}