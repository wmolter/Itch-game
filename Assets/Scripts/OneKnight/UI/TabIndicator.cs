using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class TabIndicator : MonoBehaviour {

        [Header("Color Mode")]
        public Graphic toColor;
        public Color selectedTint;

        private Color origColor;

        private TabManager manager;
        private int index;

        private void Awake() {
            origColor = toColor.color;
        }

        public void Init(TabManager manager, int index) {
            this.manager = manager;
            this.index = index;
        }

        public void Indicate(bool selected) {
            if(selected) {
                Select();
            } else {
                Deselect();
            }
        }

        private void Select() {
            toColor.color = origColor*selectedTint;
        }

        private void Deselect() {
            toColor.color = origColor;
        }

        public void OnClick() {
            if(manager != null) {
                manager.ActivateTab(index);
            }
        }
    }
}