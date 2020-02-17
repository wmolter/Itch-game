using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using OneKnight;
using OneKnight.Loading;
using OneKnight.PropertyManagement;
using OneKnight.UI;
using UnityEngine.EventSystems;

namespace Itch {
    public class PlayerManager : MonoBehaviour {

        public static PlayerManager instance;

        public PropertyManager properties;

        List<Buff> allBuffs;
        public event UnityAction<string> OnLevel;

        public int xp;
        public int totalxp;
        public float LOSLevelFactor = .2f;
        public float baseLOS = 5;
        public float LOS {
            get {
                return baseLOS + properties.AdjustProperty("Sight", abilities["Sight"])*LOSLevelFactor;
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
                return properties.AdjustProperty("Speed", baseSpeed + speedPerLevel*abilities["Speed"]);
            }
        }
        public float interactRadius;
        public ValueTextDisplay xpReadout;
        public Dictionary<string, int> abilities;
        public string[] skills;
        Inventory inventory;
        public InventoryManagerLight inventoryManager;

        public bool interacting;
        int exploreXpBuffer = 0;
        public float xpInterval = 3;

        private void Awake() {
            instance = this;
            SavingUtils.NewGame("Test");
            inventory = new Inventory(baseInvCapacity);
            abilities = new Dictionary<string, int>();
            xpReadout.toDisplay = delegate () { return xp; };
            foreach(string skill in skills) {
                abilities[skill] = 0;
            }

            properties = new PropertyManager();
            allBuffs = new List<Buff>();
            /*abilities["Mining"] = 0;
            abilities["Gathering"] = 0;
            abilities["Speed"] = 0;
            abilities["Constitution"] = 0;
            abilities["Sight"] = 0;
            abilities["Sorcery"] = 0;
            abilities["Fortitude"] = 0;*/
        }

        public Vector3Int CurrentTile { get {
                return new Vector3Int((int)(transform.position.x), (int)(transform.position.y), (int)transform.position.z);
            }
        }
        // Use this for initialization
        void Start() {
            GetComponent<Health>().max = baseHealth;
            GetComponent<Health>().current = baseHealth;
            StartCoroutine(ExploreXPCounter());
            inventoryManager.SetInventory(inventory);
            inventoryManager.AddListener(ItemClicked);
        }

        // Update is called once per frame
        void Update() {
            SqrSightRange = LOS*LOS;
        }

        private void OnDestroy() {
            instance = null;
        }

        public void ItemClicked(ItemSlot slot, PointerEventData data) {
            if(data.button == PointerEventData.InputButton.Right) {
                DropItem(slot.RemoveItem());
            }
        }

        public void MakeLevelChanges(string ability) {
            if(ability == "Constitution") {
                float newMax = baseHealth + properties.AdjustProperty(ability, abilities[ability])*healthPerLevel;
                float change = newMax - GetComponent<Health>().max;
                GetComponent<Health>().max = newMax;
                GetComponent<Health>().current = Mathf.Min(newMax, (change < 0 ? 0 : change) + GetComponent<Health>().current);
            } else if (ability == "Fortitude") {
                List<InventoryItem> leftover = inventory.SetCapacity((int)(baseInvCapacity + properties.AdjustProperty(ability, abilities[ability])));
                DropItems(leftover);
            }
        }

        public void LevelAbility(string ability) {
            int xpamount = XpToLevel(ability, abilities[ability]);
            xp -= xpamount;
            abilities[ability]++;
            totalxp += xpamount;
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

        public void OnInteract() {
            interacting = !interacting;
            if(interacting) {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
                foreach(Collider2D hit in hits) {
                    Interactable actable = hit.GetComponent<Interactable>();
                    if(actable != null) {
                        actable.Interact(this);
                    }
                }
            }
            Debug.Log("Interact called");
        }

        public void NewTiles(int count, int xpPerTile) {
            exploreXpBuffer += count*xpPerTile;
        }

        IEnumerator ExploreXPCounter() {
            while(enabled) {
                xp += exploreXpBuffer;
                exploreXpBuffer = 0;
                yield return new WaitForSeconds(xpInterval);
            }
        }

        public bool HasCapability(string ability) {
            return abilities.ContainsKey(ability);
        }

        public bool HasCapabilityLevel(string ability, float level) {
            return HasCapability(ability) && properties.AdjustProperty(ability, abilities[ability]) >= level;
        }

        class Buff {
            public string buffName { get; private set; }
            public List<float> strengths;
            public List<float> endTimes;
            public int sourceId;

            public bool Empty {
                get {
                    return strengths.Count == 0;
                }
            }

            public Buff(string name) {
                buffName = name;
                strengths = new List<float>();
                endTimes = new List<float>();
            }

            public void UpdateBuffs(PropertyManager manager, float time) {
                manager.RemoveAdjustment(buffName, sourceId);
                int index = 0;
                while(index < strengths.Count) {
                    if(endTimes[index] <= time) {
                        strengths.RemoveAt(index);
                        endTimes.RemoveAt(index);
                    } else {
                        index++;
                    }
                }
                if(strengths.Count > 0) {
                    float strengthToUse = Utils.Max(strengths);
                    manager.AddBonus(buffName, strengthToUse, sourceId);
                }
            }
        }

        public void GiveBuff(string name, float str, float dur) {
            Buff already = allBuffs.Find(delegate (Buff b) { return b.buffName == name; });
            if(already == null) {
                Buff newBuff = new Buff(name);
                already = newBuff;
                allBuffs.Add(newBuff);
            }
            already.strengths.Add(str);
            already.endTimes.Add(dur+Time.time);
            already.UpdateBuffs(properties, Time.time);
            MakeLevelChanges(name);
            StartCoroutine(CheckBuffs(dur));
        }

        IEnumerator CancelBuff(string buffName, float strength, float duration, int sourceId) {
            yield return new WaitForSeconds(duration);
            properties.RemoveAdjustment(buffName, sourceId);
            MakeLevelChanges(buffName);
        }

        IEnumerator CheckBuffs(float timeFromNow) {
            yield return new WaitForSeconds(timeFromNow);
            int index = 0;
            while(index < allBuffs.Count) { 
                allBuffs[index].UpdateBuffs(properties, Time.time);
                MakeLevelChanges(allBuffs[index].buffName);
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
            else if (remaining != item) {
                Notifications.CreateNotification(transform.position, "+" + new InventoryItem(item.ID, item.count-remaining.count).ToString());
            } else {
                Notifications.CreateNotification(transform.position, "Inventory Full");
            }
            return remaining;
        }

        public List<InventoryItem> GiveItems(ICollection<InventoryItem> items) {
            List<InventoryItem> remaining = new List<InventoryItem>();
            foreach(InventoryItem item in items) {
                InventoryItem temp = GiveItem(item);
                if(temp != null) {
                    remaining.Add(temp);
                }
            }
            return remaining;
        }

        public bool CheckConsumeFromInventory(Drop[] items) {
            if(CheckInventoryHas(items)) {
                RemoveFromInventory(items);
                return true;
            }
            return false;
        }

        public bool CheckInventoryHas(Drop[] items) {
            bool has = true;
            foreach(Drop item in items) {
                has = has && inventory.CountOf(item.id) >= item.count;
            }
            return has;
        }

        public void RemoveFromInventory(Drop[] items) {
            foreach(Drop item in items) {
                if(!inventory.RemoveAmount(item.id, item.count)) {
                    throw new UnityException("Tried to remove items that were not in the inventory: " + item);
                }
                Notifications.CreateNotification(transform.position, "-" + item.ToString());
            }
        }

        public void DropItems(List<InventoryItem> items) {
            Planes.CurrentPlane.PlaceItems(items, CurrentTile, interactRadius);
        }

        public void DropItem(InventoryItem item) {
            Planes.CurrentPlane.PlaceItem(item, CurrentTile, interactRadius);
        }
    }
}