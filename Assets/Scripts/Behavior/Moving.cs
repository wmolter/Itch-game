using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Idle")]
    public abstract class Moving : BehaviorNode {

        protected new abstract class Act : ActiveNode {
            private Moving Data { get { return (Moving)data; } }
            public Act(Moving data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            public override void OnResume(BehaviorInfo info) {
                info.move.speedFactor = Data.speedFactor;
            }

            public override void OnSuspend(BehaviorInfo info) {
                if(Data.resetMotionOnExit)
                    info.move.motionDir = Vector2.zero;
                info.move.speedFactor = 1;
            }
        }
        [Range(0, 1)]
        public float speedFactor = 1;
        public bool resetMotionOnExit = false;
        
    }
}