using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    public class TooltipManager : MonoBehaviour {

        public static TooltipManager instance;

        public GameObject container;
        public TextManipulator textDisplay;
        public InventoryItemDisplay mouseHolding;

        public void OnEnable() {
            if(instance == null)
                instance = this;
            mouseHolding.Validate();
        }

        public void Show(string text) {
            if(text != null) {
                textDisplay.Permanent(text);
                container.SetActive(true);
                //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)container.transform);
            }
        }

        public void Show(TextGetter text) {
            if(text != null) {
                textDisplay.Permanent(text);
                container.SetActive(true);
                //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)container.transform);
            }
        }

        public void Hide() {
            container.SetActive(false);
        }

        public void SetMouseHolding(InventoryItem item) {

            mouseHolding.gameObject.SetActive(item != null);
            mouseHolding.ToDisplay = item;
        }

        public void Update() {
            mouseHolding.gameObject.SetActive(mouseHolding.ToDisplay != null && SingleItemSlot.TransfersActive);
        }

    }
}