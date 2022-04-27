using UnityEngine;
using System.Collections;

namespace Itch {
    public class FleeBehavior : RelationalMovementBehavior {


        public override int GetTargetLayerMask() {
            return LayerMask.GetMask(mainControl.enemyLayers.ToArray());
        }

        public override Vector2 MoveDirection(Transform relation) {
            return transform.position - relation.position;
        }
    }
}