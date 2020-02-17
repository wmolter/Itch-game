using UnityEngine;
using System.Collections;

namespace Itch {
    public class FleeBehavior : RelationalMovementBehavior {


        public override int GetTargetLayerMask() {
            return LayerMask.GetMask(mainControl.enemyLayers);
        }

        public override Vector2 MoveDirection(Transform relation) {
            return transform.position - relation.position;
        }
    }
}