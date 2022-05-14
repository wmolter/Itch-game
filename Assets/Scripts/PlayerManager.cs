using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using OneKnight;
using OneKnight.Loading;
using OneKnight.PropertyManagement;
using OneKnight.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Itch.Effects;

namespace Itch {
    public class PlayerManager : MonoBehaviour {

        public static PlayerManager instance;

        public PropertyManager properties;

        List<Effect.State> allBuffs;
        public event UnityAction<string> OnLevel;

        public int xp;
        public int usedxp;
        public float LOSLevelFactor = .2f;
        public float baseLOS = 5;
        public float LOS {
            get {
                return properties.AdjustProperty("LOS", baseLOS + properties.AdjustProperty("Sight", abilities["Sight"])*LOSLevelFactor);
            }
        }
        public float SqrSightRange {
            get; private set;
        }
        public float healthPerLevel;
        public float baseHealth;
        public int baseInvCapacity;
        public float baseSpeed;
        public float speedPerLevel;
        public float Speed {
            get {
                return properties.AdjustProperty("Movement", baseSpeed + speedPerLevel*properties.AdjustProperty("Speed", abilities["Speed"]));
            }
        }
        public int baseHandlePoints;
        public float interactRadius;
        List<Collider2D> interactingWith;

        public TMP_ValueTextDisplay xpReadout;
        public Dictionary<string, int> abilities;
        public string[] skills;
        public List<InventoryItem> startingItems;
        Inventory inventory;
        public InventoryManagerLight inventoryManager;

        public bool interacting;
        int ExploreXPBuffer = 0;
        public float xpInterval = 3;

        private void Awake() {
            instance = this;
            SavingUtils.LoadGameData();
            SavingUtils.NewGame("Test");

            baseInvCapacity = baseInvCapacity + startingItems.Count;
            inventory = new Inventory(baseInvCapacity);
            inventory.AddStackAll(startingItems);
            abilities = new Dictionary<string, int>();
            xpReadout.SetToDisplay(delegate () { return xp; });
            foreach(string skill in skills) {
                abilities[skill] = 0;
            }

            properties = new PropertyManager();
            allBuffs = new List<Effect.State>();
            /*abilities["Mining"] = 0;
            abilities["Gathering"] = 0;
            abilities["Speed"] = 0;
            abilities["Constitution"] = 0;
            abilities["Sight"] = 0;
            abilities["Sorcery"] = 0;
            abilities["Fortitude"] = 0;*/
        }
        // Use this for initialization
        void Start() {
            GetComponent<Health>().max = baseHealth;
            GetComponent<Health>().current = baseHealth;
            StartCoroutine(ExploreXPCounter());
            inventoryManager.SetInventory(inventory);
            inventoryManager.AddListener(ItemClicked);
            OnLevel += MakeLevelChanges;

            interactingWith = new List<Collider2D>();
        }

        // Update is called once per frame
        void Update() {
            SqrSightRange = LOS*LOS;
            if(interactingWith.Count > 0) {
                for(int i = 0; i < interactingWith.Count; i++) {
                    if(interactingWith[i] == null || (interactingWith[i].ClosestPoint(transform.position)-(Vector2)transform.position).sqrMagnitude > interactRadius*interactRadius)
                        StopInteracting();
                }
            }
        }

        private void OnDestroy() {
            instance = null;
        }

        public void ItemClicked(ItemSlot slot, PointerEventData data) {
            if(data.button == PointerEventData.InputButton.Right) {
                DropItem(slot.RemoveItem());
            } else if(data.button == PointerEventData.InputButton.Left) {
                TryUseItem(slot);
            }
        }

        public void MakeLevelChanges(string ability) {
            switch(ability) {
                case "Constitution":
                    float newMax = baseHealth + properties.AdjustProperty(ability, abilities[ability])*healthPerLevel;
                    float change = newMax - GetComponent<Health>().max;
                    GetComponent<Health>().max = newMax;
                    GetComponent<Health>().current = Mathf.Min(newMax, (change < 0 ? 0 : change) + GetComponent<Health>().current);
                    break;
                case "Fortitude":
                    List<InventoryItem> leftover = inventory.SetCapacity((int)(baseInvCapacity + properties.AdjustProperty(ability, abilities[ability])));
                    DropItems(leftover);
                    break;
                case "Armor":
                    GetComponent<Health>().baseArmor = properties.AdjustProperty("Armor", abilities[ability]);
                    break;
                case "Handling":
                    GetComponent<FollowerManager>().SetHandlingCost(baseHandlePoints + Mathf.RoundToInt(properties.AdjustProperty("Handling", abilities[ability])));
                    break;
            }
        }

        public void LevelAbility(string ability) {
            int xpamount = XpToLevel(ability, abilities[ability]);
            xp -= xpamount;
            abilities[ability]++;
            usedxp += xpamount;
            OnLevel?.Invoke(ability);
        }

        public int AbilityLevel(string ability) {
            return abilities[ability];
        }

        public int XpToLevel(string ability, int level) {
            return (int)(1000f*Mathf.Pow(1.1f, level));
        }

        public int XpToLevel(string ability) {
            return XpToLevel(ability, abilities[ability]);
        }

        public bool CanLevel(string ability) {
            return xp >= XpToLevel(ability);
        }

        public void StopInteracting() {
            interactingWith.Clear();
            interacting = false;
            Debug.Log("Stop Interact Called");
        }

        public void OnInteract(InputValue value) {
            interactingWith.Clear();
            interacting = value.isPressed;
            if(interacting) {
                List<Collider2D> hits = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, interactRadius));
                int index;
                Interactable actable = null;
                do {
                    index = Utils.Closest(hits, transform.position, delegate (Collider2D c) { return c.GetComponent<Interactable>() != null; });
                    if(index >= 0) {
                        actable = hits[index].GetComponent<Interactable>();
                        interactingWith.Add(hits[index]);
                        hits.RemoveAt(index);
                    }
                } while(index >= 0 && !actable.Interact(this));
            }
            Debug.Log("Interact called");
        }

        public void NewTiles(int count, int xpPerTile) {
            ExploreXPBuffer += count*xpPerTile;
        }

        IEnumerator ExploreXPCounter() {
            while(enabled) {
                GiveXP(ExploreXPBuffer, Preferences.GetToggle("MapXPNotifs"));
                ExploreXPBuffer = 0;
                yield return new WaitForSeconds(xpInterval);
            }
        }

        public bool HasCapability(string ability) {
            return abilities.ContainsKey(ability);
        }

        public bool HasCapabilityLevel(string ability, float level) {
            return HasCapability(ability) && properties.AdjustProperty(ability, abilities[ability]) >= level;
        }

        public void GiveXP(int amount) {
            GiveXP(amount, Preferences.GetToggle("GatherXPNotifs"));
        }

        public void GiveXP(int amount, bool notif) {
            GiveXP(amount, notif, transform.position);
        }

        public void GiveXP(int amount, bool notif, Vector2 fromWhere) {
            xp += amount;
            if(notif)
                Notifications.CreateNotification(fromWhere, "+" + amount + " <b>XP</b>");
        }

        public void GiveHealth(float healing) {
            GetComponent<Health>().Heal(healing, GetComponent<Entity>());
        }

        public void Damage(float amount) {
            Damage(amount, false);
        }

        public void Damage(float amount, bool ignoreArmor) {
            GetComponent<Health>().Damage(amount, GetComponent<Entity>(), ignoreArmor);
        }

        public void StartEffect(Effect e, Vector2 notifPos) {
            Effect.State already = allBuffs.Find(e.Match);
            if(already == null) {
                Effect.State newBuff = e.Create();
                already = newBuff;
                allBuffs.Add(newBuff);
            }
            already.Add(this, e);
            StartCoroutine(CheckBuffs(e.duration));
            e.Notify(notifPos);
        }

        public void StartEffect(Effect matchWith, float duration, Vector2 notifPos) {
            Effect.State already = allBuffs.Find(matchWith.Match);
            if(already == null) {
                Effect.State newBuff = matchWith.Create();
                already = newBuff;
                allBuffs.Add(newBuff);
            }
            Effect newEffect = matchWith.Copy();
            newEffect.duration = duration;
            already.Add(this, newEffect);
            StartCoroutine(CheckBuffs(newEffect.duration));
            newEffect.Notify(notifPos);
        }

        IEnumerator CheckBuffs(float timeFromNow) {
            yield return new WaitForSeconds(timeFromNow);
            int index = 0;
            while(index < allBuffs.Count) {
                allBuffs[index].UpdateBuffs(this);
                if(allBuffs[index].Empty)
                    allBuffs.RemoveAt(index);
                else
                    index++;
            }
        }

        public InventoryItem GiveItem(InventoryItem item) {
            InventoryItem remaining = inventory.AddStackItem(item);
            if(remaining == null)
                Notifications.CreateNotification(transform.position, "+" + item.ToString());
            else if(remaining != item) {
                Notifications.CreateNotification(transform.position, "+" + new InventoryItem(item.ID, item.count-remaining.count).ToString());
            } else {
                Notifications.CreateNotification(transform.position, "Inventory Full");
            }
            return remaining;
        }

        public List<InventoryItem> GiveItems(IEnumerable<InventoryItem> items) {
            return GiveItems(items, out int count);
        }

        public List<InventoryItem> GiveItems(IEnumerable<InventoryItem> items, out int count) {
            List<InventoryItem> remaining = new List<InventoryItem>();
            count = 0;
            foreach(InventoryItem item in items) {
                InventoryItem temp = GiveItem(item);
                count++;
                if(temp != null) {
                    remaining.Add(temp);
                }
            }
            return remaining;
        }

        public bool CheckConsumeFromInventory(InventoryItem[] items) {
            return CheckConsumeFromInventory(items, out InventoryItem[] lacking);
        }

        public bool CheckConsumeFromInventory(InventoryItem[] items, out InventoryItem[] lacking) {
            if(CheckInventoryHas(items, out lacking)) {
                RemoveFromInventory(items);
                return true;
            }
            return false;
        }

        public bool CheckInventoryHas(InventoryItem[] items) {
            return CheckInventoryHas(items, out InventoryItem[] lacking);
        }

        public bool CheckInventoryHas(InventoryItem[] items, out InventoryItem[] lacking) {
            bool has = true;
            lacking = new InventoryItem[items.Length];
            int i = 0;
            foreach(InventoryItem item in items) {
                int count = inventory.CountOf(item.ID);
                has = has && count >= item.count;
                lacking[i] = new InventoryItem(item.ID, Mathf.Max(0, item.count-count));
                i++;
            }
            return has;
        }



        public void RemoveFromInventory(InventoryItem[] items) {
            foreach(InventoryItem item in items) {
                if(!inventory.RemoveAmount(item.ID, item.count)) {
                    throw new UnityException("Tried to remove items that were not in the inventory: " + item);
                }
                Notifications.CreateNotification(transform.position, "-" + item.ToString());
            }
        }

        public void DropItems(List<InventoryItem> items) {
            if(items.Count > 0)
                Planes.CurrentPlane.PlaceItems(items, GetComponent<Entity>().CurrentTile, interactRadius);
        }

        public void DropItem(InventoryItem item) {
            Planes.CurrentPlane.PlaceItem(item, GetComponent<Entity>().CurrentTile, interactRadius);
        }

        public void TryUseItem(ItemSlot slot) {
            if(!Consumables.TryUse(slot))
                Notifications.CreateError(transform.position, "Item cannot be used.");
        }
    }
}