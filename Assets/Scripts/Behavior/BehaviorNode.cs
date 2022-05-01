using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {

    public abstract class BehaviorNode : BehaviorTree<BehaviorInfo>.Node {
        protected abstract class ActiveNode : BehaviorTree<BehaviorInfo>.Node.Act {
            protected BehaviorInfo info;
            protected Transform transform { get { return info.move.transform; } }
            public ActiveNode(BehaviorNode node, ActiveNode parent, int index) : base(node, parent, index) {

            }
            public override void Init(BehaviorInfo info) {
                this.info = info;
                OnInit(info);
                foreach(ActiveNode child in children)
                    child.Init(info);
            }
            public virtual void OnInit(BehaviorInfo info) {

            }
        }

        public List<BehaviorNode> children;
        public override int ChildCount { get { return children.Count; } }
        public bool Interruptible;
        public override bool interruptible { get { return Interruptible; } }

        public override BehaviorTree<BehaviorInfo>.Node GetChild(int index) {
            return children[index];
        }

        public override Act CreateActive(Act parent, int index) {
            return CreateActive((ActiveNode)parent, index);
        }

        protected abstract ActiveNode CreateActive(ActiveNode parent, int index);
    }
}