using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace OneKnight.UI {
    public class ItemDisplayLight : MonoBehaviour, IPointerClickHandler {

        public event UnityAction<ItemSlot, PointerEventData> OnClick;
        ItemSlot slot;
        public TMP_Text text;
        // Use this for initialization

        public void Init(ItemSlot slot) {
            this.slot = slot;
            Validate();
        }

        public void Validate() {
            text.text = slot.DisplayString(1);
        }

        void Start() {

        }

        // Update is called once per frame
        void Update() {
        }

        public void OnPointerClick(PointerEventData data) {
            OnClick?.Invoke(slot, data);
            Validate();
        }
    }
}