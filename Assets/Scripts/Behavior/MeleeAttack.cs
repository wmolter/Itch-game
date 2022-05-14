using UnityEngine;
using System.Collections.Generic;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Melee Attack")]
    public class MeleeAttack : Attack {

        protected new class Act : Attack.Act {
            private MeleeAttack Data { get { return (MeleeAttack)data; } }
            public Act(MeleeAttack data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            protected override void Attack() {
                if(EnemyInRange()) {
                    ApplyAttack(info.main.behaviorTarget.GetComponent<Health>(), info.main.GetComponent<Entity>());
                }
            }

            protected void ApplyAttack(Health opponent, Entity self) {
                if(Data.effects != null)
                    Data.effects.Attack(opponent, self);
                if(Data.damage != 0)
                    new Damage() { amount=Data.damage, ignoreArmor=Data.ignoreArmor, count=1 }.Attack(opponent, self);
            }

        }
        
        public float damage;
        public bool ignoreArmor;

        public AttackEffect effects;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }

}