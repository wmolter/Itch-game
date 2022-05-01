using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Leader Search")]
    public class LeaderSearch : TargetSearch {
        protected new class Act : TargetSearch.Act {
            private LeaderSearch Data { get { return (LeaderSearch)data; } }

            public Act(LeaderSearch data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override int GetTargetLayerMask(BehaviorInfo info) {
                if(Data.searchLayers.Count == 0) {
                    return 1 << info.main.gameObject.layer;
                }
                return LayerMask.GetMask(Data.searchLayers.ToArray());
            }

            public override bool Filter(Collider2D hit) {
                TribeMember mem = hit.GetComponent<TribeMember>();
                if(mem == null)
                    return false;
                return base.Filter(hit) && mem.leader;
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}