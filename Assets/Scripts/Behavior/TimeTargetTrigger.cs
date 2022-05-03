using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Timed Target Alarm")]
    public class TimeTargetTrigger : Selector {
        protected new class Act : Selector.Act {
            private TimeTargetTrigger Data { get { return (TimeTargetTrigger)data; } }

            public Act(TimeTargetTrigger data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            private Entity prevTarget;
            private float startTime;

            public override bool Decide(BehaviorInfo info) {
                if(info.main.behaviorTarget != null & prevTarget == info.main.behaviorTarget) {
                    return Time.time >= (startTime + Data.triggerDuration) && base.Decide(info);
                } else {
                    prevTarget = info.main.behaviorTarget;
                    startTime = Time.time;
                    return false;
                }
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
        
        public float triggerDuration;
    }
}