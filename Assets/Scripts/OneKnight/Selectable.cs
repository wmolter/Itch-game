using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace OneKnight.Cameras {
    [RequireComponent(typeof(LineRenderer))]
    public class Selectable : MonoBehaviour {

        public bool showHighlight = true;
        public GameObject[] activateOnSelect;
        public string Details { get; set; }
        public string Summary { get; set; }
        public event UnityAction OnSelect;
        public event UnityAction OnDeselect;
        public UnityAction Use;
        public string UseName { get; set; }

        public Selectable Parent { get; set; }
        public bool DisplayInParent {
            get;
            set;
        }
        public List<Selectable> DisplayChildren { 
            get {
                if(Children == null)
                    return null;
                List<Selectable> toDisplay = new List<Selectable>(Children.Count);
                foreach(Selectable child in Children) {
                    if(child.DisplayInParent)
                        toDisplay.Add(child);
                }
                return toDisplay;
            }
        }
        public List<Selectable> Children { get; private set; }


        bool selected = false;
        public bool Selected {
            get {
                return selected;
            }

            set {
                selected = value;
                if(value && OnSelect != null)
                    OnSelect();
                if(!value)
                    OnDeselect?.Invoke();
                if(highlight == null)
                    highlight = GetComponent<LineRenderer>();
                highlight.enabled = selected && showHighlight;
                if(activateOnSelect != null)
                    foreach(GameObject obj in activateOnSelect)
                        obj.SetActive(selected);
            }
            
        }
        LineRenderer highlight;
        // Use this for initialization
        void Start() {
            highlight = GetComponent<LineRenderer>();
            if(highlight != null) {
                highlight.startWidth = highlight.startWidth * Mathf.Abs(transform.lossyScale.x);
                highlight.endWidth = highlight.startWidth;
            }
        }

        // Update is called once per frame
        void Update() {
            //highlight.enabled = selected;
            
        }
        
        public override string ToString() {
            return Summary;
        }

        public void AddChild(Selectable child) {
            if(Children == null) {
                Children = new List<Selectable>();
            }
            if(!Children.Contains(child)) {
                Children.Add(child);
            }/*
                if(display)
                    AddDisplayChild(child, DisplayChildren.Count);
            } else if(display){
                AddDisplayChild(child, Children.IndexOf(child));
            }*/
            child.Parent = this;
        }
        /*
        private void AddDisplayChild(Selectable child, int index) {
            if(DisplayChildren == null) {
                DisplayChildren = new List<Selectable>();
            }
            if(!DisplayChildren.Contains(child)) {
                if(index < DisplayChildren.Count)
                    DisplayChildren.Insert(index, child);
                else
                    DisplayChildren.Add(child);
            }
        }*/
    }
}
