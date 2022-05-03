using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [System.Serializable]
    public class AOEHandler {
        public float range;
        public List<string> searchLayers;
        public bool filterOut = true;
        public List<string> filterTags;

        public AOEHandler() {

        }

        public AOEHandler(AOEHandler data) {
            searchLayers = data.searchLayers;
            filterOut = data.filterOut;
            filterTags = data.filterTags;
            range = data.range;
        }

        public virtual int GetTargetLayerMask(BehaviorInfo info) {
            if(searchLayers.Count == 0) {
                return DefaultMask(info);
            }
            return LayerMask.GetMask(searchLayers.ToArray());
        }

        public virtual int DefaultMask(BehaviorInfo info) {
            return LayerMask.GetMask(info.main.enemyLayers.ToArray());
        }

        public virtual Transform FindTarget(BehaviorInfo info) {
            Collider2D[] hit = Physics2D.OverlapCircleAll(info.main.transform.position, range, GetTargetLayerMask(info));
            if(hit == null || hit.Length == 0)
                return null;
            int index = Utils.Closest(new List<Collider2D>(hit), info.main.transform.position, delegate (Collider2D col) { return Filter(col, info); });
            return index < 0 ? null : hit[index].transform;
        }

        public List<Collider2D> FindAll(BehaviorInfo info) {
            List<Collider2D> result = new List<Collider2D>();
            Collider2D[] hit = Physics2D.OverlapCircleAll(info.main.transform.position, range, GetTargetLayerMask(info));
            Utils.FilterAdd(result, hit, delegate (Collider2D col) { return Filter(col, info); });

            return result;
        }

        public virtual bool Filter(Collider2D hit, BehaviorInfo info) {
            bool result = filterTags.Contains(hit.gameObject.tag) ^ filterOut;
            return result && hit.gameObject != info.main.gameObject;
        }
    }
}