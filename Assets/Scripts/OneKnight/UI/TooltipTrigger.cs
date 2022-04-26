using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace OneKnight.UI {
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public string tooltipText;
        
        public DynamicValueFormatter dynamic;

        public void OnPointerEnter(PointerEventData data) {
            if(dynamic == null)
                TooltipManager.instance.Show(Strings.Get(tooltipText));
            else
                TooltipManager.instance.Show(dynamic.TextGetter);
        }

        public void OnPointerExit(PointerEventData data) {
            TooltipManager.instance.Hide();
        }

        public void OnDisable() {
            TooltipManager.instance.Hide();
        }
    }
}