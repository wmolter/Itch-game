using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Selector")]
    public class Selector : BehaviorNode {
        protected new class Act : ActiveNode {
            private Selector Data { get { return (Selector)data; } }
            public Act(Selector data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            public override bool Decide(BehaviorInfo info) {
                return HasWillingChild(info);
            }

            public bool TestDecide(BehaviorInfo info, Entity candidate) {

                Entity oldTarget = info.main.behaviorTarget;
                info.main.behaviorTarget = candidate;
                bool result = HasWillingChild(info);
                //Debug.Log("Test result: " + result);
                info.main.behaviorTarget = oldTarget;
                return result;
            }

            public override void OnStart(BehaviorInfo info) {

            }

            public override void DoBehavior(BehaviorInfo info) {

            }

            public override bool CheckEnd(BehaviorInfo info) {
                return !Data.checkChildrenForEnd || !HasWillingChild(info);
            }

            public override void OnFinish(BehaviorInfo info) {

            }
        }

        public bool checkChildrenForEnd;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}