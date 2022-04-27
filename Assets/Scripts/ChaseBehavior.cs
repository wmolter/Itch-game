using UnityEngine;
using System.Collections;

namespace Itch {
    public class ChaseBehavior : RelationalMovementBehavior {

        public override int GetTargetLayerMask() {
            return LayerMask.GetMask(mainControl.enemyLayers.ToArray());
        }

        public override Vector2 MoveDirection(Transform relation) {
            return relation.position - transform.position;
        }
    }
}