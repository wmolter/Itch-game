using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using OneKnight;
using OneKnight.Generation;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Gatherable : LevelRequiredAction {

        public int quantity = 1;
        public int gatherSteps = 4;
        int currentGatherSteps = 0;
        public float stepDuration = .25f;
        public bool Gathered {
            get {
                return quantity == 0;
            }
        }
        bool gathering = false;
        //guaranteed
        public Drop oneHarvestDrop;
        //chances
        public DropTable oneHarvestTable;
        [Range(1, 5)]
        public int rolls = 1;
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

        private void Start() {
            currentGatherSteps = gatherSteps;
        }

        // Update is called once per frame
        void Update() {

        }

        public void AddHarvestedResource(InventoryItem toAdd) {
            harvested.Add(toAdd);
        }

        protected override bool DoAction(PlayerManager player) {
            if(harvested != null) {
                harvested = player.GiveItems(harvested);
                CheckDepleted();
            }
            if(!Gathered) {
                if(!gathering) {
                    StartCoroutine(GatherStep(player));
                }
                return true;
            } else {
                Notifications.CreateError(transform.position, alreadyHarvested);
                return false;
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
            player.GiveXP(gatherXP);
            quantity -= 1;
            if(oneHarvestDrop.Weight != 0) {
                foreach(InventoryItem drop in oneHarvestDrop.Generate()) {
                    harvested.Add(drop);
                }
            }
            if(oneHarvestTable != null) {
                foreach(InventoryItem drop in oneHarvestTable.Generate(rolls)) {
                    if(drop != null)
                        harvested.Add(drop);
                }
            }
            if(harvested.Count > 0) { 
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