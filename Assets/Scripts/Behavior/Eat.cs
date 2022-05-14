using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Eat")]
    public class Eat : Moving {
        protected new class Act : Moving.Act {
            private Eat Data { get { return (Eat)data; } }
            public Act(Eat data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            //remember this even after finishing, so we don't go right back to eating the same thing
            private Entity current;
            private float timeLeft;
            private float startTime;

            public override void OnStart(BehaviorInfo info) {
                if(current != info.main.behaviorTarget) {
                    current = info.main.behaviorTarget;
                    timeLeft = Data.timePerMass*info.main.behaviorTarget.GetComponent<Rigidbody2D>().mass;
                }
            }

            public override void OnResume(BehaviorInfo info) {
                base.OnResume(info);
                startTime = Time.time;
                info.main.behaviorTarget = current;
            }

            public override bool Decide(BehaviorInfo info) {
                bool shouldEat = info.main.behaviorTarget != current || timeLeft > 0;
                return  Data.targetInfo.ValidTarget(info);
            }

            public override void OnSuspend(BehaviorInfo info) {
                base.OnSuspend(info);
                timeLeft -= Time.time - startTime;
            }

            public override void DoBehavior(BehaviorInfo info) {

            }

            public override void OnFinish(BehaviorInfo info) {
                if(Data.destroy && timeLeft < 0) {
                    Destroy(info.main.behaviorTarget.gameObject);
                    info.main.behaviorTarget = null;
                }
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return Time.time - startTime > timeLeft || !Data.targetInfo.ValidTarget(info);
            }
        }
        

        public TargetedHandler targetInfo;
        //maybe I should make this depend on the size of the thing.... one quick proxy would be mass
        public float timePerMass;
        public bool destroy = true;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

    }
    
}