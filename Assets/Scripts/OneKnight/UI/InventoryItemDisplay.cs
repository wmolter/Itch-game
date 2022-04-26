using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    public class InventoryItemDisplay : ImageDisplay {

        public delegate float PriceCheck();

        public TextMeshProUGUI stackDisplay;
        public bool tradeMode;
        public PriceCheck Price;

        public InventoryItem Item {
            get {
                return (InventoryItem)ToDisplay;
            }
        }

        public override void Validate() {
            base.Validate();
            if(Item != null && Item.StackLimit > 1)
                stackDisplay.text = Item.count + "";
            else
                stackDisplay.text = "";
        }

        protected override string TooltipText() {
            string result = base.TooltipText();
            if(tradeMode) {
                result += "\n\n" + Strings.Format("priceTooltip", Price());
            }
            return result;
        }
    }
}