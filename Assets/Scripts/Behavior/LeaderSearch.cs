using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Leader Search")]
    public class LeaderSearch : TargetSearch {
        public class AOE : AOEHandler {
            

            public AOE(AOEHandler data) : base (data){
            }

            public override int GetTargetLayerMask(BehaviorInfo info) {
                if(searchLayers.Count == 0) {
                    return 1 << info.main.gameObject.layer;
                }
                return LayerMask.GetMask(searchLayers.ToArray());
            }

            public override bool Filter(Collider2D hit, BehaviorInfo info) {
                TribeMember mem = hit.GetComponent<TribeMember>();
                if(mem == null)
                    return false;
                return base.Filter(hit, info) && mem.leader;
            }
        }

        protected new class Act : TargetSearch.Act {
            private LeaderSearch Data { get { return (LeaderSearch)data; } }

            public Act(LeaderSearch data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override void OnInit(BehaviorInfo info) {
                base.OnInit(info);
                Data.aoe = new AOE(Data.aoe);
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}