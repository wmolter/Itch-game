using UnityEngine;
using System.Collections.Generic;
using OneKnight.UI;


namespace OneKnight.InventoryManagement {
    public delegate void ItemEvent(ItemSlot slot);
    public class ChooseItemManager : MonoBehaviour {
        public TabManager allItemSlots;
        public TabManager invenTabs;
        public List<InventoryManager> managers;
        public ButtonEnabler chooseEnabler;

        public event ItemEvent OnChosen;

        public SingleItemSlot UISelected {
            get {
                if(allItemSlots.CurrentIndicator == null)
                    return null;
                return allItemSlots.CurrentIndicator.GetComponent<SingleItemSlot>();
            }
        }

        public ItemSlot Selected {
            get {
                if(UISelected == null)
                    return null;
                return UISelected.slot;
            }
        }

        public InventoryItem SelectedItem {
            get {
                if(UISelected == null)
                    return null;
                return UISelected.Item;
            }
        }
        // Use this for initialization
        void Start() {
            chooseEnabler.enableCheck = delegate () { return Selected != null; };
        }

        public void Choose() {
            OnChosen?.Invoke(Selected);
        }

        private void OnEnable() {
        }

        public void Init(List<string> filterCategories, params IEnumerable<ItemSlot>[] invens) {
            int i;
            for(i = 0; i < invens.Length && i < managers.Count; i++) {
                managers[i].gameObject.SetActive(true);
                managers[i].categories = filterCategories;
                managers[i].SetInventory(invens[i]);
            }
            int totalSlots = 0;
            for(int j = 0; j < i; j++) {
                totalSlots += managers[j].SlotCount;
            }
            TabIndicator[] slotSelectors = new TabIndicator[totalSlots];
            int tabIndex = 0;
            for(int j = 0; j < i; j++) {
                for(int k = 0; k < managers[j].SlotCount; k++) {
                    GameObject slot = managers[j].elementManager.ObjectAt(k);
                    TabIndicator indi = slot.GetComponent<TabIndicator>();
                    slotSelectors[tabIndex] = indi??throw new UnityException("Choose item inventory slots must have a TabIndicator.  Did you use SelectableItemSlot?");
                    tabIndex++;
                }
            }
            allItemSlots.tabIndicators = slotSelectors;
            allItemSlots.Validate();
            for(; i < managers.Count; i++) {
                invenTabs.tabIndicators[i].gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}