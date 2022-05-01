using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Target Search")]
    public class TargetSearch : Selector {
        protected new class Act : Selector.Act {
            private TargetSearch Data { get { return (TargetSearch)data; } }

            public Act(TargetSearch data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            protected Transform candidate;



            public override bool Decide(BehaviorInfo info) {
                Debug.Log("Called decide");
                candidate = FindTarget(info);
                Debug.Log("Candidate is : " + candidate);
                if(candidate != null) {
                    return TestDecide(info, candidate.GetComponent<Entity>());
                }
                return false;
            }

            public override void OnStart(BehaviorInfo info) {
                info.main.behaviorTarget = candidate.GetComponent<Entity>();
            }

            public override void DoBehavior(BehaviorInfo info) {


            }

            public override bool CheckEnd(BehaviorInfo info) {
                return true;
            }

            public override void OnFinish(BehaviorInfo info) {
                info.main.behaviorTarget = null;
                candidate = null;
            }

            public int GetTargetLayerMask(BehaviorInfo info) {
                if(Data.searchLayers.Count == 0) {
                    return LayerMask.GetMask(info.main.enemyLayers.ToArray());
                }
                return LayerMask.GetMask(Data.searchLayers.ToArray());
            }

            public virtual Transform FindTarget(BehaviorInfo info) {
                Collider2D[] hit = Physics2D.OverlapCircleAll(info.main.transform.position, Data.range, GetTargetLayerMask(info));
                if(hit == null || hit.Length == 0)
                    return null;
                int index = Utils.Closest(new List<Collider2D>(hit), info.main.transform.position, delegate (Collider2D col) { return true; });
                return hit[index].transform;
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

        public float range;
        public List<string> searchLayers;
    }
}