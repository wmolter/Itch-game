using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace OneKnight.UI {
    [RequireComponent(typeof(ReusableElement), typeof(CanvasGroup))]
    public class SidebarPopup : MonoBehaviour, IPointerEnterHandler{

        public Image icon;
        public TextMeshProUGUI textElement;

        public float fullAlphaTime = 5;
        public float fadeTime = 5;

        private float startTime;

        public string text {
            get {
                return textElement.text;
            }
            set {
                textElement.text = value;
            }
        }
        // Use this for initialization
        private void OnEnable() {
            startTime = Time.time;
        }

        public void SetIcon(string iconName) {
            Sprite newIcon = Sprites.Get(iconName);
            if(newIcon != null) {
                icon.sprite = newIcon;
            }
        }
        // Update is called once per frame
        void Update() {
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, (Time.time - (startTime + fullAlphaTime)) / fadeTime);
            if(Time.time > startTime + fullAlphaTime + fadeTime)
                Dismiss();
        }

        private void OnMouseOver() {
            startTime = Time.time;
        }

        public void OnPointerEnter(PointerEventData data) {
            startTime = Time.time;
        }

        public void Dismiss() {
            gameObject.SetActive(false);
        }
    }
}