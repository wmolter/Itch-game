using UnityEngine;
using System.Collections;

namespace Itch {
    public class FollowBehavior : RelationalMovementBehavior {
        public bool leaderRequired;
        public override int GetTargetLayerMask() {
            return 1 << gameObject.layer;
        }

        public override Transform FindTarget() {
            if(!leaderRequired)
                return base.FindTarget();
            Collider2D[] allPossible = Physics2D.OverlapCircleAll(transform.position, aggroRange, GetTargetLayerMask());
            for(int i = 0; i < allPossible.Length; i++) {
                TribeMember friend = allPossible[i].GetComponent<TribeMember>();
                if(friend != null && friend.leader)
                    return friend.transform;
            }
            return null;
        }

        public override Vector2 MoveDirection(Transform relation) {
            return relation.position - transform.position;
        }

        public override bool CheckEnd() {
            return base.CheckEnd() || !mainControl.behaviorTarget.GetComponent<TribeMember>().leader;
        }
    }
}