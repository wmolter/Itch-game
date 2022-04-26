using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextManipulator : MonoBehaviour {

        TextMeshProUGUI display;
        Color startColor;
        Coroutine currentManip;

        public float flashTime;
        public float fadeTime;
        private TextGetter textGetter;
        
        // Use this for initialization
        void Awake() {
            display = GetComponent<TextMeshProUGUI>();
            startColor = display.color;
        }

        // Update is called once per frame
        void Update() {
            UpdateText();
        }

        void UpdateText() {
            if(textGetter != null)
                display.text = textGetter();
        }

        private void OnDisable() {
            textGetter = null;
        }

        public void FlashFade(string text) {
            FlashFade(text, flashTime, fadeTime, 0);
        }

        public void FlashFade(string text, float flashTime, float fadeTime, float finalAlpha) {
            StopManipulating();
            if(display != null) {
                display.enabled = true;
                display.text = text;
                display.color = startColor;
                currentManip = StartCoroutine(FlashFadeHelper(flashTime, fadeTime, finalAlpha));
            } else {
                Debug.LogWarning("Tried to use text manipulator display before it awoke.");
            }

        }

        public void Permanent(string text) {
            display = GetComponent<TextMeshProUGUI>();
            display.enabled = true;
            display.text = text;
            //display.
        }

        public void Permanent(TextGetter text) {
            textGetter = text;
            if(display != null)
                UpdateText();
        }

        private void StopManipulating() {
            if(currentManip != null)
                StopCoroutine(currentManip);
            
        }

        private IEnumerator FlashFadeHelper(float delay, float fadeTime, float finalAlpha) {
            yield return new WaitForSeconds(delay);
            yield return UIUtils.Fade(display, finalAlpha, fadeTime);
        }

    }
}