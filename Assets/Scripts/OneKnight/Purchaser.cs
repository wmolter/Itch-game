using UnityEngine;
using System.Collections;

namespace OneKnight.InventoryManagement {
    public abstract class Purchaser : MonoBehaviour {

        public abstract bool CanBuy(float amount);
        public abstract bool CanBuy(float amount, bool alreadyAdjusted);
        public abstract bool Buy(float cost);
        public abstract bool Buy(float cost, bool alreadyAdjusted);
        public abstract void Refund(float amount);
        public abstract void Deposit(float amount);
        public abstract float AdjustedBuyPrice(float cost, Purchaser seller);
        public abstract float AdjustedSellPrice(InventoryItem item, Purchaser buyer);
    }
}