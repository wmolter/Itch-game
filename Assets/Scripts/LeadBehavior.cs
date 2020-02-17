using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(TribeMember))]
    public class LeadBehavior : SelectorBehavior {

        private void OnEnable() {
            GetComponent<TribeMember>().leader = true;
        }

        public override void OnFinish() {
            GetComponent<TribeMember>().leader = false;
        }
    }
}