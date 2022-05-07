using UnityEngine;
using System.Collections;
using OneKnight;
namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Avoid Collision")]
    public class AvoidCollision : Moving {

        protected new class Act : Moving.Act {
            private AvoidCollision Data { get { return (AvoidCollision)data; } }
            public Act(AvoidCollision data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            
            float tryAngle;
            float startTime;
            Vector2 motionDir;

            public override void OnStart(BehaviorInfo info) {
                startTime = Time.time;
                float angle = Random.Range(Data.angleRange.x, Data.angleRange.y)*Mathf.PI/180;
                motionDir = Utils.Rotate(-info.move.motionDir, angle);
            }

            public override void OnResume(BehaviorInfo info) {
                base.OnResume(info);
                info.move.motionDir = motionDir;
            }

            public override bool Decide(BehaviorInfo info) {
                //Debug.Log("Avoid collision decide called: " + info.move.StuckDuration);
                return info.move.StuckDuration >= Data.activateStuckDuration;
            }

            public override void DoBehavior(BehaviorInfo info) {
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return Time.time > startTime + Data.evadeDuration;
            }
            
        }


        public Vector2 angleRange;
        public float activateStuckDuration = 1f;
        public float evadeDuration = 1f;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}