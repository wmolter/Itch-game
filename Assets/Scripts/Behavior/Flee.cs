using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Flee")]
    public class Flee : RelationalMovement {

        protected new class Act : RelationalMovement.Act {
            private Flee Data { get { return (Flee)data; } }
            public Act(Flee data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override Vector2 MoveDirection(BehaviorInfo info, Transform relation) {
                return info.main.transform.position - relation.position;
            }
        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}