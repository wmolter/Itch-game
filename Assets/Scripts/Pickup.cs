using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Pickup : MonoBehaviour, Interaction {
        
        public int interactionPrio;
        public int Priority { get { return interactionPrio; } }
        public DropManager generator;
        public bool Generated { get { return generator.Rolled; } }
        public bool DestroyOnDeplete = false;

        private List<InventoryItem> remaining;
        // Use this for initialization

        private void OnEnable() {
            GetComponent<Interactable>().AddInteraction(this);
        }

        private void OnDisable() {
            GetComponent<Interactable>().RemoveInteraction(this);
        }
        private void Start() {
            if(enabled) {
                GetComponent<Interactable>().AddInteraction(this);
            }
        }

        public void ResetRoll() {
            if(generator!=null)
                generator.Reset();
        }

        public void AddItems(IEnumerable<InventoryItem> newItems) {
            if(remaining == null)
                remaining = new List<InventoryItem>();
            remaining.AddRange(newItems);
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
                remaining = player.GiveItems(generator.RollDrops());
                result = true;
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
            if(remaining.Count == 0) {
                if(DestroyOnDeplete)
                    Destroy(gameObject);
                else
                    enabled = false;
            }
            return result;
        }
    }
}