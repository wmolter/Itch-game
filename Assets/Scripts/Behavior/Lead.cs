using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Lead")]
    public class Lead : Selector {
        protected new class Act : Selector.Act {
            private Lead Data { get { return (Lead)data; } }
            public Act(Lead data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override void OnStart(BehaviorInfo info) {
                info.main.GetComponent<TribeMember>().leader = true;
            }

            public override void OnFinish(BehaviorInfo info) {
                info.main.GetComponent<TribeMember>().leader = false;
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}