using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Heal Self")]
    public class HealSelf : BehaviorNode {
        protected new class Act : ActiveNode {
            private HealSelf Data { get { return (HealSelf)data; } }
            private Health health { get { return info.main.GetComponent<Health>(); } }
            public Act(HealSelf data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                return health.CurrentPercentage < Data.startPercent;
            }

            public override void OnStart(BehaviorInfo info) {
                base.OnStart(info);
                health.AddRegen(Data.amount, Data.interval, Data.healingTag, info.main.GetComponent<Entity>());
            }

            public override void DoBehavior(BehaviorInfo info) {
            }

            public override void OnFinish(BehaviorInfo info) {
                base.OnFinish(info);
                health.RemoveRegen(Data.healingTag);
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return health.CurrentPercentage >= Data.endPercent;
            }
        }

        [Range(0, 1)]
        public float startPercent;
        [Range(0, 1)]
        public float endPercent;
        public float amount;
        public float interval;
        public string healingTag = "self";

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
    }
}