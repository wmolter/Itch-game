using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    [RequireComponent(typeof(TMP_Text))]
    public class TextLoader : MonoBehaviour {

        [SerializeField]
        private string textKey;
        public string Key {
            get {
                return textKey;
            }
            set {
                SetKey(value);
            }
        }

        public string text {
            get {
                return Strings.Get(textKey);
            }
        }
        // Use this for initialization
        void Start() {
            Refresh();
        }

        public void Refresh() {
            GetComponent<TMP_Text>().text = text;
        }

        public void SetKey(string key) {
            textKey = key;
            Refresh();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}