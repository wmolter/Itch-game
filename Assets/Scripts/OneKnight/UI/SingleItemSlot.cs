using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using OneKnight.InventoryManagement;

namespace OneKnight.UI {
    public class SingleItemSlot : ObjectDisplay, IPointerClickHandler {

        public static List<SingleItemSlot> transfers;

        public static bool TransfersActive {
            get {
                return transfers != null && transfers.Count > 0 || InventoryManager.transfers != null && InventoryManager.transfers.Count > 0;
            }
        }

        private static ItemSlot CursorSlot;
        public static ItemSlot TempSlotOverride;

        public static void AddTransfer(SingleItemSlot slot) {
            if(transfers == null)
                transfers = new List<SingleItemSlot>();
            transfers.Add(slot);
        }

        public static void RemoveTransfer(SingleItemSlot slot) {
            if(transfers != null)
                transfers.Remove(slot);
        }

        public static void ClearTransfers() {
            transfers = null;
        }

        public virtual ItemSlot TempSlot {
            get {
                if(TempSlotOverride == null) {
                    if(CursorSlot == null)
                        CursorSlot = new ItemSlot();
                    return CursorSlot;
                } else
                    return TempSlotOverride;
            }
        }

        public void UpdateTooltip() {
            if(TempSlot.Empty)
                TooltipManager.instance.SetMouseHolding(null);
            else
                TooltipManager.instance.SetMouseHolding(TempSlot.Item);
        }

        public string category;
        public bool overrideParent = false;
        public bool acceptsItems;
        public bool canTakeItems;
        public bool trash;
        public bool tradeMode;
        public bool validateParent;
        public InventoryItemDisplay itemDisplay;
        public ItemSlot slot {
            get {
                return (ItemSlot)ToDisplay;
            }
        }
        public InventoryManager manager;

        public InventoryItem Item {
            get {
                return slot.Item;
            }
        }

        public UnityEvent OnValidate;

        //returns null if replacing nothing; returns input if unsuccessful
        public InventoryItem ReplaceItem(InventoryItem item) {
            InventoryItem result = slot.ReplaceItem(item);
            Validate();
            return result;
        }

        public InventoryItem RemoveItem() {
            if(!canTakeItems)
                return null;
            InventoryItem result = slot.RemoveItem();
            Validate();
            return result;
        }

        public bool AddItem(InventoryItem item) {
            if(!acceptsItems)
                return false;
            bool result = slot.PutItem(item);
            Validate();
            return result;
        }
        
        

        protected virtual void Awake() {
            
            if(slot == null) {
                if(category == "") {
                    if(trash)
                        ToDisplay = new TrashSlot();
                    else
                        ToDisplay = new ItemSlot();
                } else if(trash) {
                    ToDisplay = new TrashSlot(category);
                } else { 
                    ToDisplay = new FilterItemSlot(category);
                }
            }
        }

        protected override void Start() {
            base.Start();
        }

        protected virtual void OnEnable() {
            if(manager == null && acceptsItems)
                AddTransfer(this);
        }

        protected virtual void OnDisable() {
            if(manager == null)
                RemoveTransfer(this);
        }

        public override void Validate() {
            itemDisplay.ToDisplay = slot.Item;
            OnValidate?.Invoke();
            itemDisplay.tradeMode = tradeMode;
            if(tradeMode) {
                itemDisplay.Price = delegate () { return manager.SellValue(Item); };
            }
        }
        

        public void OnPointerClick(PointerEventData data) {
            Debug.Log("Click on inventory recognized.");
            if(!canTakeItems && !acceptsItems)
                return;
            if(data.button == PointerEventData.InputButton.Left) {
                if(Input.GetButton("Modifier1"))
                    HandleShiftClick();
                else if(Input.GetButton("Modifier2"))
                    HandleControlClick();
                else
                    HandleLeftClick();
            } else if (data.button == PointerEventData.InputButton.Right) {
                HandleRightClick();
            }
            UpdateTooltip();
            if(validateParent) {
                manager.Validate();
            } else {
                Validate();
            }
        }

        public virtual void HandleShiftClick() {
            if(!canTakeItems)
                return;
            if(!TempSlot.Empty) {

            } else if(!slot.Empty) {
                Transfer();
            }
        }

        public virtual int TransferAmount() {
            return Item.count;
        }

        public virtual void Transfer() {
            int prevCount = Item.count;
            int numToTransfer = TransferAmount();
            PreTransfer(numToTransfer);
            int amount = numToTransfer;
            //prioritize transferring to separate slots, which are usually earmarked (i.e. equipment)
            if(transfers != null) {
                foreach(SingleItemSlot otherslot in transfers) {
                    Debug.Log("Transferring to : " + otherslot);
                    if(otherslot == this)
                        continue;
                    amount -= otherslot.TryStackOrAdd(slot, amount);
                    otherslot.Validate();
                    if(amount == 0)
                        break;
                }
            }
            if(amount == numToTransfer) {
                int index = 0;
                    InventoryManager.SortTransfers();
                while(index < InventoryManager.transfers.Count && TryTransfer(InventoryManager.transfers[index], amount) == 0)
                    index++;
            }
            int finalCount = slot.Empty ? 0 : Item.count;
            Debug.Log("Final count: " + finalCount + " previous count: " + prevCount + " Number to transfer: " + numToTransfer + "Not transferred: " + (amount));
            PostTransfer(numToTransfer - (prevCount - finalCount));
        }

        public virtual void PostTransfer(int notTransferred) {

        }

        public virtual void PreTransfer(int amountToTransfer) {

        }

        public virtual int TryTransfer(InventoryManager newManager, int count) {

            if(newManager != null && newManager != manager) {
                Debug.Log("Transferring to " + newManager);
                return newManager.Stack(slot, count);
            }
            return 0;
        }

        public virtual void HandleControlClick() {

        }

        public virtual void HandleRightClick() {
            if(!TempSlot.Empty) {
                if(HandleSameItem()) ;
                else if(acceptsItems) {
                    if(TryStackOne()) ;
                    else {
                        TryAddOne();
                    }
                }
                //Debug.Log("Moved item: " + ItemOnCursor.item + " to inventory: " + this);
            } else if(canTakeItems) {
                TakeHalf();
            }
        }

        public virtual void HandleLeftClick() {
            Debug.Log("Left clicked on inventory.");
            if(!TempSlot.Empty) {
                Debug.Log("TempSlot is not empty.");
                if(acceptsItems) {
                    if(TryStackOrAdd() > 0) Debug.Log("Stacked on.");
                    else if(canTakeItems) {
                        TrySwap();
                    } else {
                        Debug.Log("Accepts items but nothing worked.");
                    }
                }
                //Debug.Log("Moved item: " + ItemOnCursor.item + " to inventory: " + this);
            } else if(canTakeItems) {
                TakeItem();
            }
        }

        public bool TryStackOne() {
            if(Item == null)
                return false;
            if(slot.CanStack(TempSlot.Item)) {
                InventoryItem result = slot.Stack(TempSlot.RemoveOne());
                if(result == null)
                    return true;
                else {
                    TempSlot.Stack(result);
                    return false;
                }
            } else {
                return false;
            }
        }

        public int TryStackOrAdd() {
            return TryStackOrAdd(TempSlot);
        }

        public virtual int TryStackOrAdd(ItemSlot otherslot) {
            Debug.Log("TryStackOrAdd called with count: " + otherslot.Item.count);
            return TryStackOrAdd(otherslot, otherslot.Item.count);
        }

        public virtual int TryStackOrAdd(ItemSlot otherslot, int max) {
            if(!acceptsItems)
                return 0;
            if(otherslot.Empty)
                return 0;
            int result = otherslot.StackOnto(slot, max);
            return result;
        }

        public bool TrySwap() {
            return slot.Swap(TempSlot);
        } 

        public bool TryAddToEmpty() {
            return slot.Swap(TempSlot);

        }

        public bool TryAddOne() {
            InventoryItem one = TempSlot.RemoveOne();
            if(AddItem(one)) {
                Debug.Log("Added one item to this slot: " + Item);
                return true;
            } else {
                TempSlot.Stack(one);
                return false;
            }
        }

        public bool HandleSameItem() {
            /*if(ItemOnCursor.source == this && ItemOnCursor.item == Item) {
                ItemOnCursor.item = null;
                return true;
            }*/
            return false;
        }

        public void TakeItem() {
            InventoryItem remaining = TempSlot.Stack(RemoveItem());
            slot.Stack(remaining);
            Debug.Log("Added item to cursor: " + TempSlot.Item);
        }

        public void TakeHalf() {
            if(slot.Empty)
                return;
            InventoryItem remaining = TempSlot.Stack(slot.SplitStack());
            slot.Stack(remaining);
            Debug.Log("Took half: " + TempSlot.Item);
        }

        public void TakeOne() {
            InventoryItem remaining = TempSlot.Stack(slot.RemoveOne());
            slot.Stack(remaining);
            Debug.Log("Took one: " + TempSlot.Item);
        }

        public void Take(int number) {
            InventoryItem remaining = TempSlot.Stack(slot.Remove(number));
            slot.Stack(remaining);
        }

    }
}