using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Landed Trigger")]
    public class LandedTrigger : Selector {
        protected new class Act : Selector.Act {
            private LandedTrigger Data { get { return (LandedTrigger)data; } }

            public Act(LandedTrigger data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                return info.main.GetComponent<JumpMovement>().landedFlag && base.Decide(info);
            }

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                info.main.GetComponent<JumpMovement>().landedFlag = false;
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}