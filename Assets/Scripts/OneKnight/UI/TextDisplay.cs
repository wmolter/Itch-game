using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    public class TextDisplay : ObjectDisplay {
        public TextMeshProUGUI output;

        public override void Validate() {
            output.text = ToDisplay.ToString();
        }

    }
}