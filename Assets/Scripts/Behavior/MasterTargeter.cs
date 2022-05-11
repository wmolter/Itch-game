using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Master Targeter")]
    public class MasterTargeter : Selector {
        protected new class Act : Selector.Act {
            private MasterTargeter Data { get { return (MasterTargeter)data; } }
            public Act(MasterTargeter data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            private Entity master;
            public override bool Decide(BehaviorInfo info) {
                Tamed tamed = info.main.GetComponent<Tamed>();
                if(tamed == null || !tamed.follow)
                    return false;
                master = tamed.manager.GetComponent<Entity>();
                return Data.targetInfo.WouldBeValid(info, master) && TestDecide(info, master);
            }

            public override void OnStart(BehaviorInfo info) {
                info.main.behaviorTarget = master;
            }

            public override void OnFinish(BehaviorInfo info) {
                info.main.behaviorTarget = null;
            }

            public override void DoBehavior(BehaviorInfo info) {
            }

            public override bool CheckEnd(BehaviorInfo info) {
                return !Data.targetInfo.ValidTarget(info) || base.CheckEnd(info);
            }
        }
        

        public TargetedHandler targetInfo;

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

    }
    
}