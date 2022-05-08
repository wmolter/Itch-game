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
                candidate = Data.aoe.FindTarget(info);
                if(info.main.gameObject.name.Contains("Yeti")) {
                    Debug.Log("Yeti candidate: " + candidate);
                }
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

            public override void OnFinish(BehaviorInfo info) {
                info.main.behaviorTarget = null;
                candidate = null;
            }
        }

        public AOEHandler aoe;


        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}