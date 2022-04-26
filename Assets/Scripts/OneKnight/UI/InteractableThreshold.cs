using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    [RequireComponent(typeof(Button))]
    public class InteractableThreshold : MonoBehaviour {
        
        public float threshold;
        public float value;

        public ValueTextDisplay thresholdReference;
        public ValueTextDisplay valueReference;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if(thresholdReference != null)
                threshold = (float)thresholdReference.toDisplay();
            if(valueReference != null)
                value = (float)valueReference.toDisplay();
            GetComponent<Button>().interactable = value >= threshold;
        }
    }
}