using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    public abstract class Attack : Moving {
        protected new abstract class Act : Moving.Act {
            private Attack Data { get { return (Attack)data; } }
            public Act(Attack data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            float nextAttackTime;
            float fireTime;
            protected bool preparing;

            public override void OnInit(BehaviorInfo info) {
                base.OnInit(info);
                nextAttackTime = Time.time;
            }

            public override bool Decide(BehaviorInfo info) {
                /*bool can = CanAttack();
                bool inRange = EnemyInRange();
                Debug.Log("Attack decide called. CanAttack: " + can + " inRange: " + inRange);
                return can && inRange;*/
                return CanAttack() && EnemyInRange();
            }

            public virtual bool EnemyInRange() {
                return Data.targetInfo.ValidTarget(info);
            }

            public bool CanAttack() {
                return Time.time >= nextAttackTime;
            }

            public override void DoBehavior(BehaviorInfo info) {
                if(!preparing && CanAttack()) {
                    info.move.speedFactor = Data.speedDuringAttack;
                    preparing = true;
                    fireTime = Time.time + Data.attackTime;
                }
                if(preparing && fireTime <= Time.time) {
                    Attack();
                    preparing = false;
                    nextAttackTime = Time.time + Data.cooldown;
                    info.move.speedFactor = Data.speedFactor;
                }
            }

            protected abstract void Attack();

            public override bool CheckEnd(BehaviorInfo info) {
                return (!Data.lockDuringCooldown || CanAttack()) && !preparing && (Data.returnOnCooldown || !EnemyInRange());
            }
        }

        public float attackTime;
        public float cooldown;
        public TargetedHandler targetInfo;
        [Range(0, 1)]
        public float speedDuringAttack;
        public bool returnOnCooldown = true;
        public bool lockDuringCooldown = false;

        
    }
}