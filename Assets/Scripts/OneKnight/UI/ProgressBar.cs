using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    [RequireComponent(typeof(Image))]
    public class ProgressBar : MonoBehaviour {

        private Image loadBar;
        public float startFill;
        public bool autoUpdate = false;
        public Utils.FloatGetter toDisplay;
        // Use this for initialization
        void Awake() {
            loadBar = GetComponent<Image>();
            loadBar.fillAmount = startFill;
        }
        
        public void SetProgress(float percent) {
            loadBar.fillAmount = percent;
            //loadBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, percent*startSize);
        }

        private void Update() {
            if(autoUpdate)
                SetProgress(toDisplay());
        }
    }
}
