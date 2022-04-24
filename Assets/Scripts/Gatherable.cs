using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using OneKnight;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Gatherable : MonoBehaviour {

        public int quantity = 1;
        public int gatherSteps = 4;
        int currentGatherSteps = 0;
        public float stepDuration = .25f;
        public int gatherXP = 0;
        public bool Gathered {
            get {
                return quantity == 0;
            }
        }
        bool gathering = false;
        public string capabilityRequired = "Mining";
        public float levelRequired = 0;
        public Drop[] oneHarvest;
        public string buffName = "";
        public float buffStrength = 1;
        public float buffDuration = 10;
        public Sprite depletedSprite;
        public bool destroyOnDeplete;
        List<InventoryItem> harvested;

        [Header("strings")]
        public string alreadyHarvested = "No resources left.";
        // Use this for initialization
        void Awake() {
            harvested = new List<InventoryItem>();
        }

        private void OnEnable() {
            GetComponent<Interactable>().Interaction = Gather;
        }

        private void Start() {
            currentGatherSteps = gatherSteps;
        }

        // Update is called once per frame
        void Update() {

        }

        public void AddHarvestedResource(InventoryItem toAdd) {
            harvested.Add(toAdd);
        }

        public void Gather(PlayerManager player) {
            if(harvested != null) {
                harvested = player.GiveItems(harvested);
                CheckDepleted();
            }
            if(!Gathered) {
                if(!gathering) {
                    if(player.HasCapabilityLevel(capabilityRequired, levelRequired)) {
                        StartCoroutine(GatherStep(player));
                    } else {
                        Notifications.CreateError(transform.position, capabilityRequired + " " + levelRequired + " Required");
                    }
                }
            } else {
                Notifications.CreateError(transform.position, alreadyHarvested);
            }
        }

        IEnumerator GatherStep(PlayerManager player) {
            gathering = true;
            ProgressHint hint = Notifications.CreateHint(transform.position, (gatherSteps - currentGatherSteps)*1f/gatherSteps);
            while(currentGatherSteps > 0) {
                Debug.Log("Gather steps: " + currentGatherSteps);
                yield return new WaitForSeconds(stepDuration);
                if(player.interacting) {
                    currentGatherSteps--;
                    hint.SetProgress((gatherSteps - currentGatherSteps)*1f/gatherSteps);
                } else {
                    hint.Dismiss();
                    gathering = false;
                    yield break;
                }
            }
            hint.Dismiss();
            gathering = false;
            currentGatherSteps = gatherSteps;
            player.xp += gatherXP;
            quantity -= 1;
            if(oneHarvest.Length > 0) {
                foreach(Drop drop in oneHarvest) {
                    harvested.Add(new InventoryItem(drop));
                }
                harvested = player.GiveItems(harvested);
                foreach(InventoryItem item in harvested) {
                    Notifications.CreateNotification(transform.position, item.ToString() + " Remaining");
                }
            }
            if(buffName != "") {
                player.GiveBuff(buffName, buffStrength, buffDuration, transform.position);
            }
            CheckDepleted();
        }

        public void CheckDepleted() {
            if(harvested.Count == 0 && quantity == 0) {
                GetComponent<SpriteRenderer>().sprite = depletedSprite;
                if(destroyOnDeplete) {
                    Destroy(gameObject);
                }
            }
        }
    }
}