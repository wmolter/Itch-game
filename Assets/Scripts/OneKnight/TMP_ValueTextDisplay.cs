using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace OneKnight {

    [RequireComponent(typeof(TMP_Text))]
    public class TMP_ValueTextDisplay : MonoBehaviour {

        public string valueName;
        public bool displayName;
        public string unit;
        public bool useFormat;
        public string format;

        public ValueGetter toDisplay;
        public delegate System.IFormattable ValueGetter();
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void LateUpdate() {
            string displayString = displayName ? valueName + " " : "";
            displayString += useFormat? string.Format("{0:" + format + "}", toDisplay()) : toDisplay().ToString();
            displayString += unit;
            GetComponent<TMP_Text>().text = displayString;
        }
    }
}