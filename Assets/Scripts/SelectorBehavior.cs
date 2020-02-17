using UnityEngine;
using System.Collections;

namespace Itch {
    public class SelectorBehavior : BehaviorTreeNode {
        public override void DoBehavior() {
        }

        public override bool CheckEnd() {
            return true;
        }

        public override bool Decide() {
            return HasWillingChild();
        }
    }
}