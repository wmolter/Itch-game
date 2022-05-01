using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Self Defense")]
    public class SelfDefense: BehaviorNode {
        protected new class Act : ActiveNode {
            private SelfDefense Data { get { return (SelfDefense)data; } }
            public Act(SelfDefense data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            Entity angryAt;
            public override bool Decide(BehaviorInfo info) {
                return angryAt != null;
            }

            public override void OnInit(BehaviorInfo info) {
                info.main.GetComponent<Health>().OnDamaged += NoticeAttack;
            }

            public override void OnStart(BehaviorInfo info) {
                info.main.behaviorTarget = angryAt;
                info.main.enemyLayers.Add(LayerMask.LayerToName(angryAt.gameObject.layer));
            }

            public override void DoBehavior(BehaviorInfo info) {

            }

            public override bool CheckEnd(BehaviorInfo info) {
                return true;
            }

            public override void OnFinish(BehaviorInfo info) {
                info.main.enemyLayers.Remove(LayerMask.LayerToName(angryAt.gameObject.layer));
                angryAt = null;
            }

            private void NoticeAttack(Health.EventData data) {
                if(data.byWho != null && data.byWho.gameObject != info.main.gameObject) {
                    if(data.percentage <= Data.percentageThreshold && (data.byWho.transform.position - info.main.transform.position).magnitude <= Data.noticeRange) {
                        angryAt = data.byWho;
                    }
                }
            }
        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
        //a long ranged attack may not trigger this behavior
        public float noticeRange = 10;
        //at 1, any damage will trigger this
        [Range(0, 1)]
        public float percentageThreshold = 1;
        

    }
}