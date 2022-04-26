using UnityEngine;
using System.Collections;
using OneKnight.InventoryManagement;

namespace OneKnight.UI {
    public class TradeSlot : SingleItemSlot {

        public int rightClickAmount;
        public Purchaser buyer;
        [SerializeField]
        private Purchaser seller;
        private InventoryItem originalItem;

        protected override void Start() {
            base.Start();
        }

        public override void Validate() {
            base.Validate();
            itemDisplay.Price = delegate () {
                return ActualCostOne(Item.count);
            };
        }

        public override void HandleLeftClick() {
            if(!TempSlot.Empty) {
                if(acceptsItems && CanSell(TempSlot.Item)) {
                    TryStackOrAdd();
                }
                //Debug.Log("Moved item: " + ItemOnCursor.item + " to inventory: " + this);
            } else if(canTakeItems && !slot.Empty) {
                TryBuyOne(Item.count);
            }
        }

        public override void HandleRightClick() {
            if(!TempSlot.Empty) {
                if(acceptsItems) {
                    TrySellOne();
                }
                //Debug.Log("Moved item: " + ItemOnCursor.item + " to inventory: " + this);
            } else if(canTakeItems) {
                TryBuy(rightClickAmount);
            }
        }

        public void SetSeller(Purchaser seller) {
            this.seller = seller;
            originalItem = Item == null ? null : Item.Copy();
        }

        public float CostPer() {
            return buyer.AdjustedBuyPrice(seller.AdjustedSellPrice(Item, buyer), seller);
        }

        public float SellValuePer(InventoryItem item) {
            return seller.AdjustedBuyPrice(buyer.AdjustedSellPrice(item, seller), buyer);
        }

        public bool CanSell(InventoryItem item) {
            return true;
        }

        public override int TransferAmount() {
            int result = MaxBuy();
            Debug.Log("Trade transfer amount: " + result);
            return result;
        }

        public override void PreTransfer(int amountToTransfer) {
            for(int i = 0; i < amountToTransfer; i++) {
                buyer.Buy(ActualCostOne(Item.count - i), true);
            }
        }

        public override void PostTransfer(int notTransferred) {
            for(int i = 0; i < notTransferred; i++) {
                buyer.Deposit(ActualCostOne(Item.count-i));
            }
        }

        public override int TryStackOrAdd(ItemSlot otherslot, int max) {
            int prevCount = Item == null ? 0 : Item.count;
            int result = base.TryStackOrAdd(otherslot, max);
            int finalCount = Item == null ? 0 : Item.count;
            for(int i = prevCount; i < finalCount; i++) {
                SellOne(i);
            }
            return result;
        }

        public void TrySellOne() {
            if(TryStackOne()) {
                SellOne(Item.count - 1);
            } else if(TryAddOne()) {
                SellOne(Item.count - 1);
            }
        }

        private void SellOne(int count) {
            //this happens after it is stacked, so use leq
            float price;
            if(originalItem == null) {
                price = SellValuePer(Item);
            } else if(count < originalItem.count && Item.ID == originalItem.ID) {
                //Sellback - but really this should work for all slots
                price = CostPer();
            } else {
                price = SellValuePer(Item);
            }
            buyer.Deposit(price);
        }

        public void TryBuy(int amount) {
            for(int i = 0; i < amount && !slot.Empty; i++) {
                if(!TryBuyOne(Item.count))
                    return;
            }
        }

        public float ActualCostOne(int count) {

            float cost;
            if(originalItem == null || originalItem.ID != Item.ID || originalItem.ID == Item.ID && originalItem.count < count) {
                //buyback price
                cost = SellValuePer(Item);
            } else {
                //normal buying price
                cost = CostPer();
            }
            return cost;
        }

        public int MaxBuy() {
            int amount = 0;
            float cost = 0;
            while(buyer.CanBuy(cost, true) && amount <= Item.count) {
                cost += ActualCostOne(Item.count - amount);
                amount++;
            }
            amount--;
            return amount;
        }

        public bool TryBuyOne(int count) {
            if(Item == null)
                return false;
            float cost = ActualCostOne(count);
            if(buyer.CanBuy(cost, true)) {
                buyer.Buy(cost, true);
                TakeOne();
                return true;
            } else {
                return false;
            }
        }
    }
}