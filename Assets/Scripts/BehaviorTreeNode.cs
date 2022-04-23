using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public abstract class BehaviorTreeNode : MonoBehaviour {

        public Enemy mainControl;
        public Movement moveControl;
        public List<BehaviorTreeNode> children;
        BehaviorTreeNode parent;
        public bool interruptible;
        public bool parentRedecidesOnExit;
        int activeChild = int.MaxValue;
        // Use this for initialization
        void Start() {
            foreach(BehaviorTreeNode child in children) {
                child.parent = this;
                child.mainControl = mainControl;
                if(child.moveControl == null)
                    child.moveControl = moveControl;
            }
        }

        // Update is called once per frame
        void Update() {
            DoBehavior();
            if(!DecideChildren() && CheckEnd()) {
                enabled = false;
                OnFinish();
                activeChild = children.Count;
                ReturnControlToParent();
            }
        }

        void ReturnControlToParent() {
            parent.activeChild = parent.children.Count;
            if(!parentRedecidesOnExit || parent.Decide())
                parent.enabled = true;
            else
                parent.ReturnControlToParent();
        }

        bool DecideChildren() {
            bool result = false;
            //only interrupt for higher priority
            for(int i = 0; i < children.Count && i < activeChild; i++) {
                if(children[i].Decide()) {
                    enabled = children[i].interruptible;
                    result = true;
                    activeChild = i;
                    break;
                }
            }
            for(int i = 0; i < children.Count; i++) {
                if(i != activeChild) {
                    if(children[i].enabled)
                        children[i].OnFinish();
                    children[i].DisableChildren();
                }
                children[i].enabled = i == activeChild;
            }
            //an already active child should count as working
            result |= activeChild < children.Count;
            return result;
        }

        public bool HasWillingChild() {
            bool willing = false;
            for(int i = 0; i < children.Count; i++) {
                willing |= children[i].Decide();
            }
            return willing;
        }

        public int FirstWillingChild() {
            for(int i = 0; i < children.Count; i++) {
                if(children[i].Decide())
                    return i;
            }
            return children.Count;
        }

        public void DisableChildren() {
            for(int i = 0; i < children.Count; i++) {
                if(children[i].enabled)
                    children[i].OnFinish();
                children[i].enabled = false;
            }
            activeChild = children.Count;
        }

        public abstract bool Decide();

        public abstract void DoBehavior();

        public abstract bool CheckEnd();

        public virtual void OnFinish() {

        }
    }
}