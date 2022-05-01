using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Move with Target")]
    public class MoveWithTarget : BehaviorNode {
        protected new class Act : ActiveNode {
            private MoveWithTarget Data { get { return (MoveWithTarget)data; } }
            public Act(MoveWithTarget data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            public override bool Decide(BehaviorInfo info) {
                return info.main.behaviorTarget != null && !CheckEnd(info);
            }

            public override void DoBehavior(BehaviorInfo info) {
                info.move.motionDir = (MoveDirection(info, info.main.behaviorTarget.transform)).normalized;
            }

            public override bool CheckEnd(BehaviorInfo info) {
                float sqr = Vector2.SqrMagnitude(info.main.behaviorTarget.transform.position - info.main.transform.position);
                return sqr > Data.maxRange*Data.maxRange || sqr < Data.minRange*Data.minRange;
            }

            public override void OnSuspend(BehaviorInfo info) {
                info.move.motionDir = Vector2.zero;
            }

            public Vector2 MoveDirection(BehaviorInfo info, Transform relation) {
                return Utils.Rotate(relation.position - info.main.transform.position, Data.moveAngle*Mathf.PI/180).normalized;
            }
        }

        [Range(-180, 180)]
        public float moveAngle = 0;
        public float minRange, maxRange;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

    }
    
}