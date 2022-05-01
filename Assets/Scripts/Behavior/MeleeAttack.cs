using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Melee Attack")]
    public class MeleeAttack : Attack {

        protected new class Act : Attack.Act {
            private MeleeAttack Data { get { return (MeleeAttack)data; } }
            public Act(MeleeAttack data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            protected override void Attack() {
                if(EnemyInRange()) {
                    info.main.behaviorTarget.GetComponent<Health>().Damage(Data.damage, info.main.GetComponent<Entity>());
                }
            }

        }
        
        public float damage;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }

}