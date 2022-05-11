using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [System.Serializable]
    public class BehaviorInfo {
        public Enemy main;
        public Movement move;
    }

    public class BehaviorTree : BehaviorTree<BehaviorInfo> {
        public BehaviorNode root;
        public override Node Root { get { return root; } }
        public BehaviorInfo info;
        public override BehaviorInfo Info { get { return info; } }

        protected override void Awake() {
            base.Awake();
            activeRoot.Init(info);
        }

        protected override void OnSetRoot(Node root) {
            this.root = (BehaviorNode)root;
            activeRoot.Init(info);
        }

        public void ValidateInfo() {
            Movement[] moves = GetComponents<Movement>();
            int i = 0;
            while(i < moves.Length-1 && moves[i].enabled == false) {
                i++;
                info.move = moves[i];
            }
            //i don't know why there would be more than one of these, but hey, i added another movement, so who knows....
            Enemy[] es = GetComponents<Enemy>();
            i = 0;
            while(i < es.Length-1 && es[i].enabled == false) {
                i++;
                info.main = es[i];
            }
        }
    }
}