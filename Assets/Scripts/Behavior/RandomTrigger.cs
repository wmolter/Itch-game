using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Random Trigger")]
    public class RandomTrigger : Selector {
        protected new class Act : Selector.Act {
            private RandomTrigger Data { get { return (RandomTrigger)data; } }

            public Act(RandomTrigger data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            private float nextCheckTime = 0;
            private bool decision = false;

            public override bool Decide(BehaviorInfo info) {
                if(Time.time < nextCheckTime)
                    return decision;
                if(nextCheckTime == 0) {
                    nextCheckTime = Time.time + Data.checkInterval;
                    if(!Data.checkFirstTime)
                        return false;
                } else
                    nextCheckTime = Time.time + Data.checkInterval - ((Time.time - nextCheckTime) % Data.checkInterval);
                bool result = Random.value < Data.chance && base.Decide(info);
                decision = result;
                //Debug.Log("Random trigger decide called, result: " + result + " next check time: " + nextCheckTime + " Time: " + Time.time);
                return result;
            }

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                //potentially want this as an option?
                decision = false;
            }

        }
        public float checkInterval;
        [Range(0, 1)]
        public float chance;
        public bool checkFirstTime = true;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}