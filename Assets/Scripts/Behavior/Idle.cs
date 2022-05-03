using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Idle")]
    public class Idle : BehaviorNode {

        protected new class Act : ActiveNode {
            private Idle Data { get { return (Idle)data; } }
            public Act(Idle data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            float nextChangeTime = 0;

            public override bool Decide(BehaviorInfo info) {
                return true;
            }

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                ChangeDir(info.move);
            }
            public override void OnResume(BehaviorInfo info) {
                info.move.speedFactor = Data.speedFactor;
                if(Data.continueInDir) {
                    UpdateChangeTime();
                } else {
                    nextChangeTime = Time.time;
                }
            }

            void ChangeDir(Movement m) {

                if(Random.value < Data.standstillChance) {
                    m.motionDir = Vector2.zero;
                } else if(m.motionDir == Vector2.zero) {
                    m.motionDir = Random.insideUnitCircle.normalized;
                } else {
                    float currAngle = Vector2.SignedAngle(Vector2.right, m.motionDir);
                    float newAngle = currAngle + Random.Range(-Data.angleChangeRange, Data.angleChangeRange);
                    float radians = newAngle*Mathf.PI/180;
                    Vector2 newDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                    //Debug.Log(name + " Old angle: " + currAngle + " New angle: " + newAngle + " Old motion dir: " + moveControl.motionDir + " New motion dir: " + newDir);

                    m.motionDir = newDir;
                }
            }

            public override void DoBehavior(BehaviorInfo info) {
                if(Time.time >= nextChangeTime) {
                    ChangeDir(info.move);
                    UpdateChangeTime();
                }
            }

            protected void UpdateChangeTime() {
                nextChangeTime = Time.time + Random.Range(Data.durationRange.x, Data.durationRange.y);
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return false;
            }

            public override void OnSuspend(BehaviorInfo info) {
                if(Data.resetMotionOnExit)
                    info.move.motionDir = Vector2.zero;
                info.move.speedFactor = 1;
            }

            public override void OnFinish(BehaviorInfo info) {
                nextChangeTime = Time.time;
            }
        }

        public float standstillChance = .5f;
        public Vector2 durationRange = new Vector2(2, 5);
        [Range(0.01f, 1)]
        public float speedFactor = 1;
        public float angleChangeRange = 90;
        public bool continueInDir = true;
        public bool resetMotionOnExit = false;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}