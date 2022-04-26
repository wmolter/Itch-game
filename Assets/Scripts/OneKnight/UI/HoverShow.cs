using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class HoverShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public CanvasGroup toChange;
        public float minAlpha;
        public float maxAlpha;
        public bool startMax;
        public bool fadeOnEnter;
        public float fadeTime;
        private Coroutine ongoingFade;
        // Use this for initialization
        void OnEnable() {
            if(startMax) {
                toChange.alpha = maxAlpha;
            } else {
                toChange.alpha = minAlpha;
            }
        }

        // Update is called once per frame
        void Update() {

        }

        public IEnumerator Fade(float startAlpha, float endAlpha, float seconds) {
            float startTime = Time.time;
            while(startTime + seconds > Time.time) {
                toChange.alpha = Mathf.Lerp(startAlpha, endAlpha, (Time.time-startTime)/seconds);
                yield return null;
            }
            Debug.Log("Changed group alpha to: " + endAlpha);
            toChange.alpha = endAlpha;
        }

        public void OnPointerEnter(PointerEventData data) {
            if(ongoingFade != null)
                StopCoroutine(ongoingFade);
            Debug.Log("Pointer entered hovershow.");
            if(fadeOnEnter)
                ongoingFade = StartCoroutine(Fade(minAlpha, maxAlpha, fadeTime));
            else
                ongoingFade = StartCoroutine(Fade(minAlpha, maxAlpha, 0));
        }

        public void OnPointerExit(PointerEventData data) {
            ongoingFade = StartCoroutine(Fade(maxAlpha, minAlpha, fadeTime));
        }
    }
}