using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [System.Serializable]
    public struct BehaviorInfo {
        public Enemy main;
        public Movement move;
    }

    public class BehaviorTree : BehaviorTree<BehaviorInfo> {
        public BehaviorNode root;
        public override Node Root { get { return root; } }
        public BehaviorInfo info;
        public override BehaviorInfo Info { get { return info; } }

        protected override void Start() {
            base.Start();
            activeRoot.Init(info);
        }
    }
}