using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

namespace OneKnight.UI {
    [RequireComponent(typeof(TMP_Text))]
    public class FontSizeFix : MonoBehaviour {

        public int size;

        private void Start() {
            GetComponent<TMP_Text>().fontSize = size;
            GetComponent<TMP_Text>().SetLayoutDirty();
        }
    }
}