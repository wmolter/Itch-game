using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace OneKnight.UI {
    public abstract class ObjectHoverDisplay : ObjectDisplay, IPointerExitHandler, IPointerEnterHandler {



        public void OnPointerExit(PointerEventData data) {
            TooltipManager.instance.Hide();
        }


        public virtual void OnPointerEnter(PointerEventData data) {

            Displayable detailed = (Displayable)ToDisplay;
            if(detailed != null)
                TooltipManager.instance.Show(TooltipText());
        }

        protected virtual string TooltipText() {
            Displayable detailed = (Displayable)ToDisplay;
            return detailed.DisplayString(5);
        }
    }
}