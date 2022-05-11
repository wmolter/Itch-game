using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Teleport")]
    public class Teleport : BehaviorNode {

        protected new class Act : ActiveNode {
            private Teleport Data { get { return (Teleport)data; } }
            public Act(Teleport data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                return info.main.behaviorTarget != null;
            }

            public override void DoBehavior(BehaviorInfo info) {
                Vector2 goTo = info.main.behaviorTarget.transform.position;
                Vector2 curr = info.main.transform.position;
                Vector2 result = goTo + (curr-goTo).normalized * Data.destDistance;
                info.main.transform.position = result;
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return true;
            }
        }

        public float destDistance = 1;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}