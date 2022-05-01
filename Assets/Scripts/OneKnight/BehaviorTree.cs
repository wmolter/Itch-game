using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
namespace OneKnight {
    public abstract class BehaviorTree<T> : MonoBehaviour {

        public abstract Node Root { get; }
        public Node.Act activeRoot { get; private set; }
        public Node.Act current { get; private set; }

        public abstract T Info { get; }
        // Use this for initialization
        protected virtual void Start() {
            
            current = activeRoot = Root.CreateActive(null, 0);
        }

        // Update is called once per frame
        void Update() {
            Node.Act interrupt = ChooseInterrupt();
            if(interrupt != current) {
                Debug.Log("Behavior interrupted: " + current + " by " + interrupt);
                SetCurrent(interrupt);
            } else if(current.CheckEnd(Info)) {
                current.OnFinish(Info);
                SetCurrent(ChooseNext());
            }
            current.DoBehavior(Info);
        }

        Node.Act ChooseNext() {
            Node.Act result = current.parent;
            //go up tree
            while(result.CheckEnd(Info)) {
                result.OnFinish(Info);
                Debug.Log("Behavior just ended: " + current);
                result = result.parent;
                if(result == null) {
                    throw new UnityException("No willing behavior on exit: " + gameObject.name + " last behavior: " + current.name);
                }
            }
            return DownTree(result);
        }

        Node.Act DownTree(Node.Act start) {

            //go back down tree
            int index = start.FirstWillingChild(Info);
            while(index < start.children.Count) {
                start = start.children[index];
                start.OnStart(Info);
            }
            return start;
        }

        //as is now, 2 things are potentially weird:
        //OnFinish is called for a parent whose child interrupts it
        //after an interruption finishes, we redecide everthing, rather than returning
        Node.Act ChooseInterrupt() {
            //go up the tree
            Node.Act result = current;
            Node.Act active = current;
            while(active.interruptible) {
                int index = active.parent.FirstWillingChild(Info);
                if(index < active.indexInParent) {
                    active.OnFinish(Info);
                    result = active.parent.children[index];
                }
                active = active.parent;
            }

            //children always can "interrupt" a behavior (they count as part of that behavior!)
            if(result != current)
                result.OnStart(Info);
            return DownTree(result);
        }

        void SetCurrent(Node.Act next) {
            current.OnSuspend(Info);
            current = next;
            next.OnResume(Info);
            Debug.Log("New behavior started: " + next);
        }

        public void OnDrawGizmos() {
            GUIStyle style = new GUIStyle();
            Handles.Label(transform.position, current.name);
        }

        [System.Serializable]
        public abstract class Node: ScriptableObject {
            public abstract class Act {
                public List<Act> children;
                public Act parent { get; private set; }
                protected Node data;
                public bool interruptible { get { return data.interruptible; } }
                public int indexInParent { get; private set; }
                public string name { get { return data.name; } }

                public Act(Node data, Act parent, int index) {
                    this.data = data;
                    this.indexInParent = index;
                    this.parent = parent;
                    children = new List<Act>(data.ChildCount);
                    for(int i = 0; i < data.ChildCount; i++) {
                        children.Add(data.GetChild(i).CreateActive(this, i));
                    }
                }

                public bool HasWillingChild(T info) {
                    bool willing = false;
                    for(int i = 0; i < children.Count; i++) {
                        willing |= children[i].Decide(info);
                    }
                    return willing;
                }

                public int FirstWillingChild(T info) {
                    for(int i = 0; i < children.Count; i++) {
                        if(children[i].Decide(info))
                            return i;
                    }
                    return children.Count;
                }
                public abstract void Init(T info);
                public abstract bool Decide(T info);

                public virtual void OnStart(T info) {

                }

                public abstract void DoBehavior(T info);

                public abstract bool CheckEnd(T info);

                public virtual void OnFinish(T info) {

                }
                public virtual void OnSuspend(T info) {

                }
                public virtual void OnResume(T info) {

                }
            }

            public abstract Act CreateActive(Act parent, int index);

            //these are abstract because unity does not serialize generified types
            public abstract int ChildCount { get; }
            public abstract Node GetChild(int i);
            public abstract bool interruptible { get; }
            // Use this for initialization
            
        }
    }
}