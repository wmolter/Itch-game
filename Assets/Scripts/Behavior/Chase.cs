using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Chase")]
    public class Chase : RelationalMovement {
        protected new class Act : RelationalMovement.Act {

            public Act(Chase data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override Vector2 MoveDirection(BehaviorInfo info, Transform relation) {
                return relation.position - info.main.transform.position;
            }
        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}