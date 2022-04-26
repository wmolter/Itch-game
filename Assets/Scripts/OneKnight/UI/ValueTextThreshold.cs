using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    [RequireComponent(typeof(TMP_ValueTextDisplay))]
    public class ValueTextThreshold : MonoBehaviour {

        public float upperThreshold;
        public float lowerThreshold;
        public Gradient withinRangeColors;
        private Color origColor;
        // Use this for initialization
        void Start() {
            origColor = GetComponent<TextMeshProUGUI>().color;
        }

        // Update is called once per frame
        void LateUpdate() {
            float value = (float)GetComponent<TMP_ValueTextDisplay>().GetMainValue();
            GetComponent<TextMeshProUGUI>().color = withinRangeColors.Evaluate((value-lowerThreshold)/(upperThreshold-lowerThreshold));
        }
    }
}