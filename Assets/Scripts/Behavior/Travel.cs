using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName ="Behavior/Travel")]
    public class Travel : Moving {
        protected new class Act : Moving.Act {
            private Travel Data { get { return (Travel)data; } }
            public Act(Travel data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            bool resting = false;
            float nextEndTime;
            Vector2 travelDirection;

            public override void OnInit(BehaviorInfo info) {
                float angle = Mathf.PI/180*Random.Range(Data.minAngle, Data.maxAngle);
                nextEndTime =  Time.time;
                travelDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }

            public override bool Decide(BehaviorInfo info) {
                //if we got interrupted, we might not be resting
                return !resting || Time.time >= nextEndTime;
            }

            public override void DoBehavior(BehaviorInfo info) {
                if(Time.time >= nextEndTime) {
                    if(!resting && Random.value < Data.restChance) {
                        info.move.motionDir = Vector2.zero;
                        nextEndTime = Random.Range(Data.restDurationRange.x, Data.restDurationRange.y) + Time.time;
                        resting = true;
                    } else {
                        resting = false;
                        float travelAngle = Vector2.SignedAngle(Vector2.right, travelDirection);
                        float newAngle = travelAngle + Random.Range(-Data.angleDeviation, Data.angleDeviation);
                        float radians = newAngle*Mathf.PI/180;
                        info.move.motionDir = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
                        nextEndTime = Random.Range(Data.travelDurationRange.x, Data.travelDurationRange.y);
                    }
                }
            }
            public override bool CheckEnd(BehaviorInfo info) {
                return resting;
            }
        }

        public float restChance = .5f;
        public Vector2 travelDurationRange = new Vector2(20, 30);
        public Vector2 restDurationRange = new Vector2(20, 30);
        [Range(-180, 180)]
        public float minAngle = -180;
        [Range(-180, 180)]
        public float maxAngle = 180;
        [Range(0, 45)]
        public float angleDeviation;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }


    }
}