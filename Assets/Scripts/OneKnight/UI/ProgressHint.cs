using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class ProgressHint : MonoBehaviour {

        public Image under;
        public Image main;
        public Text cornerText;
        // Use this for initialization
        void Start() {

        }

        public void SetProgress(float progress) {
            main.fillAmount = progress;
        }

        public void Dismiss() {
            Destroy(gameObject);
        }
    }
}