using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Move with Target")]
    public class MoveWithTarget : Moving {
        protected new class Act : Moving.Act {
            private MoveWithTarget Data { get { return (MoveWithTarget)data; } }
            public Act(MoveWithTarget data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            private float timeLeft;
            private float startTime;
            Entity current;

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                if(current != info.main.behaviorTarget) {
                    current = info.main.behaviorTarget;
                    timeLeft = Data.boredDuration;
                }
                startTime = Time.time;
            }

            public override bool Decide(BehaviorInfo info) {
                return (timeLeft > 0 || info.main.behaviorTarget != current) && Data.targetInfo.ValidTarget(info);
            }

            public override void DoBehavior(BehaviorInfo info) {
                info.move.motionDir = (MoveDirection(info, info.main.behaviorTarget.transform)).normalized;
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return Time.time - startTime > timeLeft || !Data.targetInfo.ValidTarget(info);
            }

            public override void OnFinish(BehaviorInfo info) {
                base.OnFinish(info);
                timeLeft -= Time.time - startTime;
            }

            public Vector2 MoveDirection(BehaviorInfo info, Transform relation) {
                return Utils.Rotate(relation.position - info.main.transform.position, Data.moveAngle*Mathf.PI/180).normalized;
            }
        }
        

        public TargetedHandler targetInfo;
        [Range(-180, 180)]
        public float moveAngle = 0;

        public float boredDuration = 60;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

    }
    
}