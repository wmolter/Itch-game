using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Return Home")]
    public class ReturnHome : Moving {
        protected new class Act : Moving.Act {
            private ReturnHome Data { get { return (ReturnHome)data; } }
            public Act(ReturnHome data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                return Data.allowedRange*Data.allowedRange < Vector2.SqrMagnitude(info.main.transform.position - info.main.Home);
            }

            public override void DoBehavior(BehaviorInfo info) {
                info.move.motionDir = (info.main.Home - info.main.transform.position).normalized;
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return Vector2.SqrMagnitude(info.main.transform.position - info.main.Home) < Data.returnCompleteRange*Data.returnCompleteRange;
            }
        }

        public float allowedRange;
        public float returnCompleteRange;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}