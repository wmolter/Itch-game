using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/AOE Attack")]
    public class AOEAttack : MeleeAttack {
        public class AOE : AOEHandler {


            public AOE(AOEHandler data) : base(data) {
            }

            public override bool Filter(Collider2D hit, BehaviorInfo info) {
                Health hp = hit.GetComponent<Health>();
                if(hp == null)
                    return false;
                return base.Filter(hit, info) && hp.Alive;
            }
        }

        protected new class Act : MeleeAttack.Act {
            private AOEAttack Data { get { return (AOEAttack)data; } }
            public Act(AOEAttack data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            public override void OnInit(BehaviorInfo info) {
                base.OnInit(info);
                Data.aoe = new AOE(Data.aoe);
            }

            public override bool EnemyInRange() {
                return Data.aoe.FindAll(info).Count > 0;
            }

            protected override void Attack() {
                //Debug.Log("AOE attack called.");
                foreach(Collider2D aoeHit in Data.aoe.FindAll(info)) {
                    //Debug.Log("Damaging: " + aoeHit.gameObject.name);
                    ApplyAttack(aoeHit.GetComponent<Health>(), info.main.GetComponent<Entity>());
                }
            }

        }
        
        public AOEHandler aoe;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }

}