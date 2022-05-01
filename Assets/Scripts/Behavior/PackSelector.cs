using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Pack")]
    public class PackSelector : Selector {
        protected new class Act : Selector.Act {
            private PackSelector Data { get { return (PackSelector)data; } }
            public Act(PackSelector data, ActiveNode parent, int index) : base(data, parent, index) {

            }
            bool shouldLead = false;

            Collider2D[] tribeMates;
            Entity targetToCopy;
            public override bool Decide(BehaviorInfo info) {
                bool packActing = false;
                shouldLead = false;
                int willingChildIndex = FirstWillingChild(info);
                tribeMates = Physics2D.OverlapCircleAll(info.main.transform.position, Data.packRange, 1 << info.main.gameObject.layer);
                //can put any number higher than length for leaderActivityIndex if we want all activities to be leader
                if(tribeMates.Length >= Data.numberNeeded && willingChildIndex <= Mathf.Min(Data.leaderActivityIndex, children.Count-1)) {
                    shouldLead = true;
                    return true;
                } else {
                    foreach(Collider2D mate in tribeMates) {
                        TribeMember buddy = info.main.GetComponent<TribeMember>();
                        if(buddy != null) {
                            if(buddy.teamActivity) {
                                targetToCopy = buddy.GetComponent<Enemy>().behaviorTarget;
                            }
                            packActing |= buddy.teamActivity;
                        }
                    }
                }

                Debug.Log("Pack selector decide called: " + packActing + " shouldLead: " + shouldLead);
                return packActing && TestDecide(info, targetToCopy);
            }

            public override void OnStart(BehaviorInfo info) {
                info.main.GetComponent<TribeMember>().leader = shouldLead;
                if(!shouldLead) {
                    info.main.behaviorTarget = targetToCopy;
                }
                info.main.GetComponent<TribeMember>().teamActivity = true;
            }

            public override void OnFinish(BehaviorInfo info) {
                base.OnFinish(info);
                info.main.GetComponent<TribeMember>().leader = false;
                info.main.GetComponent<TribeMember>().teamActivity = false;
                targetToCopy = null;
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
        public float packRange;
        public int numberNeeded;
        public int leaderActivityIndex;


    }
}