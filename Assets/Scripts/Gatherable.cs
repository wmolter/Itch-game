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

        public string buffName = "";
        public float buffStrength = 1;
        public float buffDuration = 10;
        public Sprite depletedSprite;

        [Header("strings")]
        public string alreadyHarvested = "No resources left.";
        // Use this for initialization
        void Awake() {
        }

        private void Start() {
            currentGatherSteps = gatherSteps;
        }

        // Update is called once per frame
        void Update() {

        }
        protected override bool DoAction(PlayerManager player) {
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

            Pickup oneHarvestTable = GetComponent<Pickup>();
            if(oneHarvestTable != null) {
                oneHarvestTable.ResetRoll();
                oneHarvestTable.enabled = true;
                oneHarvestTable.Interact(player);
            }

            if(buffName != "") {
                player.GiveBuff(buffName, buffStrength, buffDuration, transform.position);
            }
            CheckDepleted();
        }

        public void CheckDepleted() {
            if(quantity == 0) {
                GetComponent<SpriteRenderer>().sprite = depletedSprite;
            }
        }
    }
}