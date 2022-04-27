using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public class TargetSearch : BehaviorTreeNode {
        public float range;
        public List<string> searchLayers;
        protected Transform candidate;

        public override bool Decide() {
            candidate = FindTarget();
            return candidate != null;
        }

        private void OnEnable() {
            mainControl.behaviorTarget = candidate.GetComponent<Entity>();
        }

        public override void DoBehavior() {

        }

        public override bool CheckEnd() {
            return true;
        }

        public override void OnFinish() {
            mainControl.behaviorTarget = null;
            candidate = null;
        }

        public int GetTargetLayerMask() {
            return LayerMask.GetMask(searchLayers.ToArray());
        }

        public virtual Transform FindTarget() {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, range, GetTargetLayerMask());
            if(hit == null)
                return null;
            return hit.transform;
        }
    }
}