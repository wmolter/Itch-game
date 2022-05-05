using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Activate Component")]
    public class ComponentActivate : BehaviorNode {
        protected new class Act : ActiveNode {
            private ComponentActivate Data { get { return (ComponentActivate)data; } }
            private MonoBehaviour comp { get { return ((MonoBehaviour)info.main.GetComponent(Data.componentType)); } }
            
            public Act(ComponentActivate data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                Debug.Log("Component activate decide called: " + comp);
                if(comp == null)
                    return false;
                return !comp.enabled;
            }

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                comp.enabled = true;
            }

            public override void DoBehavior(BehaviorInfo info) {
            }

            public override void OnFinish(BehaviorInfo info) {
                base.OnFinish(info);
                if(comp != null)
                    comp.enabled = false;
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return (Data.useParentEnd && parent.CheckEnd(info)) || !Data.lockWhileEnabled || (comp == null || !comp.enabled);
            }
        }

        public string componentType = "Trigger";
        public bool lockWhileEnabled;
        public bool useParentEnd;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}