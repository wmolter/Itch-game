using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(TribeMember))]
    public class PackSelector : SelectorBehavior {

        public float packRange;
        public int numberNeeded;
        public bool packActing { get; private set; }
        public int leaderActivityIndex;

        Collider2D[] tribeMates;
        public override bool Decide() {
            int willingChildIndex = FirstWillingChild();
            if(willingChildIndex >= children.Count)
                return false;
            tribeMates = Physics2D.OverlapCircleAll(transform.position, packRange, 1 << gameObject.layer);
            if(tribeMates.Length >= numberNeeded && willingChildIndex <= leaderActivityIndex) {
                packActing = true;
                GetComponent<TribeMember>().leader = true;
                return true;
            } else {
                packActing = false;
                GetComponent<TribeMember>().leader = false;
                foreach(Collider2D mate in tribeMates) {
                    PackSelector packBehavior = mate.GetComponent<PackSelector>();
                    if(packBehavior != null) {
                        packActing |= packBehavior.packActing;
                    }
                }
                return packActing;
            }
        }

        public override void OnFinish() {
            base.OnFinish();
            GetComponent<TribeMember>().leader = false;
        }

    }
}