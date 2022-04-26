using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {

    public delegate bool Check();

    [RequireComponent(typeof(Button))]
    public class ButtonEnabler : MonoBehaviour {

        public Check enableCheck;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if(enableCheck != null) {
                GetComponent<Button>().interactable = enableCheck();
            }
        }
    }
}