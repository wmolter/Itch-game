using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {

    public static class UIUtils {

        public static IEnumerator Fade(Graphic display, float finalAlpha, float seconds) {
            float endTime = Time.time + seconds;
            Color fromColor = display.color;
            while(Time.time < endTime) {
                float percentage = (endTime - Time.time)/seconds;
                display.color = new Color(fromColor.r, fromColor.g, fromColor.b, Mathf.Lerp(finalAlpha, fromColor.a, percentage));
                yield return null;
            }
            display.color = new Color(fromColor.r, fromColor.g, fromColor.b, finalAlpha);
        }
    }
}