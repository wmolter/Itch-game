using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    public abstract class Attack : BehaviorNode {
        protected new abstract class Act : ActiveNode {
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

            public bool EnemyInRange() {
                if(info.main.behaviorTarget == null)
                    return false;
                ColliderDistance2D dist = info.main.GetComponent<Collider2D>().Distance(info.main.behaviorTarget.GetComponent<Collider2D>());
                //Debug.Log("Enemy in Range called: " + info.main.behaviorTarget + " and distance: " + dist.distance);
                return dist.distance < Data.range;//Physics2D.OverlapCircle(info.move.transform.position, Data.fireRange, LayerMask.GetMask(info.main.enemyLayers.ToArray())) != null;
            }

            public bool CanAttack() {
                return Time.time >= nextAttackTime;
            }

            public override void DoBehavior(BehaviorInfo info) {
                info.move.motionDir = Vector2.zero;
                if(!preparing && CanAttack()) {
                    preparing = true;
                    fireTime = Time.time + Data.attackTime;
                }
                if(preparing && fireTime <= Time.time) {
                    Attack();
                    preparing = false;
                    nextAttackTime = Time.time + Data.cooldown;
                }
            }

            protected abstract void Attack();

            public override bool CheckEnd(BehaviorInfo info) {
                return !preparing && (Data.returnOnCooldown || !EnemyInRange());
            }
        }

        public float attackTime;
        public float cooldown;
        public float range = 5;
        public bool returnOnCooldown = true;

        
    }
}