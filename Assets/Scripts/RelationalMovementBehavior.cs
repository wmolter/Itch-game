using UnityEngine;
using System.Collections;

namespace Itch {
    public abstract class RelationalMovementBehavior : BehaviorTreeNode {
        public float aggroRange;
        public float endAggroRange;
        protected Transform toChase, candidate;

        public override bool Decide() {
            if(toChase != null)
                return true;
            candidate = FindTarget();
            return candidate != null;
        }

        private void OnEnable() {
            toChase = candidate;
        }

        public override void DoBehavior() {
            mainControl.motionDir = (MoveDirection(toChase.transform)).normalized;

        }

        public override bool CheckEnd() {
            return Vector2.SqrMagnitude(toChase.transform.position - transform.position) > endAggroRange*endAggroRange;
        }

        public override void OnFinish() {
            toChase = null;
            candidate = null;
            mainControl.motionDir = Vector2.zero;
        }

        public abstract Vector2 MoveDirection(Transform relation);
        public abstract int GetTargetLayerMask();

        public virtual Transform FindTarget() {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, aggroRange, GetTargetLayerMask());
            if(hit == null)
                return null;
            return hit.transform;
        }
    }
    
}