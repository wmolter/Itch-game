using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneKnight.InventoryManagement;
using UnityEngine.Events;

namespace OneKnight.UI {
    [RequireComponent(typeof(Button))]
    public class PurchaseButton : MonoBehaviour {


        public ValueTextDisplay costDisplay;
        public Purchaser purchaser;
        public UnityEvent OnPurchase;
        public Utils.FloatGetter CostGetter;
        public float cost;

        public float Cost {
            get {
                if(CostGetter == null)
                    return cost;
                return CostGetter();
            }
        }
        // Use this for initialization
        void Awake() {
            costDisplay.toDisplay = delegate () { return Cost; };
        }
        

        // Update is called once per frame
        void Update() {
            GetComponent<Button>().interactable = purchaser.CanBuy(Cost);
        }

        public void TryPurchase() {
            if(purchaser.Buy(Cost)) {
                OnPurchase?.Invoke();
            }
        }
    }
}