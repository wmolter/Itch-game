using UnityEngine;
using System.Collections;
using OneKnight.UI;

namespace Itch {
    public class HealthBarManager : MonoBehaviour {

        public ProgressBar bar;
        public TMP_ValueTextDisplay text;

        public Health toDisplay;

        private void Start() {
            bar.toDisplay = toDisplay.GetPercentage;
            text.SetToDisplay(toDisplay.GetCurrentUI, toDisplay.GetMaxUI);
        }
    }
}