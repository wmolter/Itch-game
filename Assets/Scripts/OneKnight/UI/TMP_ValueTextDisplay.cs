using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace OneKnight.UI {

    [RequireComponent(typeof(TMP_Text))]
    public class TMP_ValueTextDisplay : MonoBehaviour {
        
        public bool useFormat;
        public string format;

        ValueGetter[] toDisplay;
        System.IFormattable[] values;
        public delegate System.IFormattable ValueGetter();
        // Use this for initialization
        void Start() {
        }

        public void SetToDisplay(params ValueGetter[] toDisplay) {
            this.toDisplay = toDisplay;
            values = new System.IFormattable[toDisplay.Length];
        }

        public System.IFormattable GetMainValue() {
            return toDisplay[0]();
        }

        // Update is called once per frame
        void LateUpdate() {
            for(int i = 0; i < values.Length; i++) {
                values[i] = toDisplay[i]();
            }
            string displayString = useFormat? string.Format(format, values) : toDisplay[0].ToString();
            GetComponent<TMP_Text>().text = displayString;
        }
    }
}