using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Follower")]
    public class FollowerEdit : BehaviorNode {
        protected new class Act : BehaviorNode.ActiveNode {
            private FollowerEdit Data { get { return (FollowerEdit)data; } }
            private Tamed tamed { get { return info.main.GetComponent<Tamed>(); } }
            private FollowerManager master { get { return tamed.manager; } }
            public Act(FollowerEdit data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                return tamed != null && master != null;
            }

            public override void OnStart(BehaviorInfo info) {
                if(master == null || !master.ToggleFollower(tamed))
                    info.main.GetComponent<FollowerManager>()?.TryAddFollower(tamed);
            }

            public override void DoBehavior(BehaviorInfo info) {
            }
            

            public override bool CheckEnd(BehaviorInfo info) {
                return true;
            }
        }
        

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}