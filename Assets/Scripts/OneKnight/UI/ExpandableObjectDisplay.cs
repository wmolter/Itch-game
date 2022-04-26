using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace OneKnight.UI {
    [RequireComponent(typeof(ObjectDisplay))]
    public class ExpandableObjectDisplay : MonoBehaviour {

        public ExpandableOption expander;
        public ObjectDisplay childPrefab;
        public bool recursiveMode;
        public UnityAction<object> OnAction;

        private bool populated;
        // Use this for initialization
        void Start() {
            expander.OnPreExpand += Populate;
            expander.OnAction += ExpanderToggled;
        }

        private void OnDestroy() {
            expander.OnPreExpand -= Populate;
            expander.OnAction -= ExpanderToggled;
        }

        public void OnBecameInvisible() {
            ResetChildren();
        }
        

        public void ResetChildren() {
            if(populated) {
                populated = false;
                foreach(Transform child in expander.transform)
                    Destroy(child.gameObject);
            }
        }

        private void ExpanderToggled() {
            if(OnAction != null)
                OnAction(GetComponent<ObjectDisplay>().ToDisplay);
        }

        public void Populate() {
            //Debug.Log("Called Populate with " + ((KnowledgeUI.InfoWithChildren)(GetComponent<ObjectDisplay>().ToDisplay)).children.Count);
            if(!populated) {
                foreach(object child in (IEnumerable)(GetComponent<ObjectDisplay>().ToDisplay)) {
                    ObjectDisplay disp = Instantiate(childPrefab, expander.transform);
                    disp.ToDisplay = child;
                    disp.gameObject.SetActive(true);
                    if(disp.GetComponent<ExpandableObjectDisplay>() != null) {
                        disp.GetComponent<ExpandableObjectDisplay>().OnAction = OnAction;
                        if(recursiveMode)
                            disp.GetComponent<ExpandableObjectDisplay>().childPrefab = childPrefab;
                    }
                }
                populated = true;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}