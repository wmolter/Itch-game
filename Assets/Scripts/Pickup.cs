using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Pickup : MonoBehaviour, Interaction {
        
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public DropManager generator;
        public bool Generated { get; private set; }
        public bool DestroyOnDeplete = false;
        public bool useLocks = true;
        private List<object> locks;
        private List<object> Locks { get {
                if(locks == null)
                    locks = new List<object>();
                return locks;
            } }

        [Header("strings")]
        public string noLootError = "No loot this time.";

        private List<InventoryItem> remaining;
        // Use this for initialization

        private void Awake() {
            remaining = new List<InventoryItem>();
        }

        private void OnEnable() {
            GetComponent<Interactable>().AddInteraction(this);
            TryRoll();
        }

        private void OnDisable() {
            GetComponent<Interactable>().RemoveInteraction(this);
        }
        private void Start() {
            if(enabled) {
                GetComponent<Interactable>().AddInteraction(this);
            }
        }

        public void AddLock(object o) {
            if(!Locks.Contains(o))
                Locks.Add(o);
        }

        public void RemoveLock(object o) {
            Locks.Remove(o);
        }

        private void TryRoll() {
            if(generator != null && !Generated)
                AddItems(Roll());
            CheckDisableDestroy();
        }

        private IEnumerable<InventoryItem> Roll() {
            Generated = true;
            return generator.RollDrops();
        }

        public void ResetRoll() {
            Generated = false;
        }

        public void AddItems(IEnumerable<InventoryItem> newItems) {
            AddItems(newItems, out int count);
        }

        public void AddItems(IEnumerable<InventoryItem> newItems, out int count) {
            if(remaining == null)
                remaining = new List<InventoryItem>();
            count = remaining.Count;
            remaining.AddRange(newItems);
            count = remaining.Count - count;
            InventoryItem.CompressList(remaining, false);
        }

        public void AddItem(InventoryItem item) {
            if(remaining == null) {
                remaining = new List<InventoryItem>();
            }
            remaining.Add(item);
            InventoryItem.CompressList(remaining, false);
        }

        // Update is called once per frame
        void Update() {

        }

        public bool Interact(PlayerManager player) {
            bool result = false;
            if(generator != null && !Generated) {
                AddItems(player.GiveItems(Roll(), out int count));
                //if no items were dropped, act like this ~never happened~
                result = count > 0;
                if(count == 0)
                    Notifications.CreateError(transform.position, noLootError);
            } else {
                int startCount = remaining.Count;
                remaining = player.GiveItems(remaining);
                //so technically, you can fill stacks without triggering this, but it's annoying to find
                //any *changed* items, and the result (that we also interact with something else nearby)
                //is not gamebreaking
                result = startCount > remaining.Count;
            }
            foreach(InventoryItem item in remaining) {
                Notifications.CreateNotification(transform.position, item.ToString() + " Remaining");
            }
            CheckDisableDestroy();
            return result;
        }

        private void CheckDisableDestroy() {
            if(remaining.Count == 0) {
                if(DestroyOnDeplete && !(useLocks && Locks.Count > 0))
                    Destroy(gameObject);
                else
                    enabled = false;
            }

        }
    }
}