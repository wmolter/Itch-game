using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch {
    public class MoveWithTarget : BehaviorTreeNode {
        [Range(0, 180)]
        public float moveAngle = 0;
        public float endRange;

        public override bool Decide() {
            return mainControl.behaviorTarget != null;
        }

        private void OnEnable() {
        }

        public override void DoBehavior() {
            moveControl.motionDir = (MoveDirection(mainControl.behaviorTarget.transform)).normalized;

        }

        public override bool CheckEnd() {
            return Vector2.SqrMagnitude(mainControl.behaviorTarget.transform.position - transform.position) > endRange*endRange;
        }

        public override void OnFinish() {
            moveControl.motionDir = Vector2.zero;
        }

        public Vector2 MoveDirection(Transform relation) {
            return Utils.Rotate(relation.position - transform.position, moveAngle*Mathf.PI/180).normalized;
        }
    }
    
}