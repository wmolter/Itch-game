using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using OneKnight;
using OneKnight.Generation;
using OneKnight.UI;

namespace Itch {
    public class CraftGather : LevelRequiredAction {

        
        private CraftGatherData d { get { return (CraftGatherData)data; } }

        public InventoryItem[] materialsNeeded { get { return d.materialsNeeded; } }
        public int quantity { get { return d.quantity; } }
        int remainingQuantity;
        public int gatherSteps { get { return d.gatherSteps; } }
        int currentGatherSteps = 0;
        public float stepDuration { get { return d.stepDuration; } }
        public bool Gathered {
            get {
                return remainingQuantity == 0;
            }
        }
        bool gathering = false;
        float totalCraftTime { get { return gatherSteps*stepDuration; } }
        float craftTime { get { return currentGatherSteps*stepDuration; } }

        public DropManager oneHarvestTable { get { return d.oneHarvestTable; } }


        public MonoBehaviour unlockedComponent { get { return (MonoBehaviour)GetComponent(d.unlockedComponentName); } }
        public Sprite finishedSprite { get { return d.finishedSprite; } }
        public string finishedAnimationState { get { return d.finishedAnimationState; } }
        public Vector2 hintOffset { get { return d.hintOffset; } }
        
        public string alreadyHarvested { get { return d.alreadyHarvested; } }

        float finishTime;
        // Use this for initialization
        void Awake() {
            //this must stay in awake so when both are inactive...
            if(oneHarvestTable != null) {
                GetComponentInParent<Pickup>().AddLock(this);
            }
            if(GetComponentInParent<Interactable>() == null)
                Debug.LogError("CraftGather component has no interactable parent. It will do nothing.");
        }
        // Update is called once per frame
        void Update() {

        }

        protected override bool DoAction(PlayerManager player) {
            if(!Gathered) {
                if(!gathering) {
                    InventoryItem[] lacking;
                    if(player.CheckInventoryHas(materialsNeeded, out lacking)) {
                        StartCoroutine(Craft(player));
                        return true;
                    } else {
                        Notifications.CreateError(transform.position, "Materials missing: " + Strings.ItemList(lacking));
                        return false;
                    }
                    //StartCoroutine(GatherStep(player));
                }
                return true;
            } else {
                Notifications.CreateError(transform.position, alreadyHarvested);
                return false;
            }
        }

        IEnumerator Craft(PlayerManager player) {
            gathering = true;
            ProgressHint hint = Notifications.CreateHint(transform, hintOffset);
            ProgressHint littleHint = null;
            if(gatherSteps > 1) {
                littleHint = Notifications.CreateHelperHint(transform, hintOffset);
            }
            finishTime = Time.time + craftTime;
            while(Time.time < finishTime) {
                currentGatherSteps = Mathf.CeilToInt((finishTime-Time.time)/stepDuration);
                UpdateHints(hint, littleHint, finishTime - Time.time);
                if(player.interacting)
                    yield return null;
                else {
                    hint.Dismiss();
                    littleHint?.Dismiss();
                    gathering = false;
                    Debug.Log("Craft gather paused at: " + currentGatherSteps);
                    yield break;
                }
            }
            gathering = false;
            currentGatherSteps = gatherSteps;
            if(player.interacting) {
                InventoryItem[] lacking;
                if(player.CheckConsumeFromInventory(materialsNeeded, out lacking)) {
                    CraftFinished(player);

                } else {
                    Notifications.CreateError(transform.position, "Materials missing: " + Strings.ItemList(lacking));
                }
            }
            hint.Dismiss();
            littleHint?.Dismiss();
        }

        void UpdateHints(ProgressHint main, ProgressHint helper, float totalTimeLeft) {
            if(helper == null)
                main.SetProgress(1 - (totalTimeLeft/totalCraftTime));
            else {
                main.SetProgress((gatherSteps - currentGatherSteps)*1f/gatherSteps);
                helper.SetProgress(1 - (totalTimeLeft%stepDuration)/stepDuration);
            }
        }

        void CraftFinished(PlayerManager player) {

            player.GiveXP(d.gatherXP);
            remainingQuantity -= 1;
            if(Gathered) {
                if(unlockedComponent != null)
                    unlockedComponent.enabled = true;
                Animator anim = GetComponent<Animator>();
                if(anim != null) {
                    anim.Play(finishedAnimationState);
                } else if(finishedSprite != null)
                    GetComponent<SpriteRenderer>().sprite = finishedSprite;

                //allow this to now be destroyed
                if(oneHarvestTable != null)
                    GetComponentInParent<Pickup>()?.RemoveLock(this);
                //maybe we leave this as an option? but with interaction prio it shouldn't be a problem for enterables
                if(d.disableOnFinish)
                    enabled = false;
            }

            foreach(Effect e in d.effects) {
                player.StartEffect(e, player.transform.position);
            }

            if(oneHarvestTable != null) {
                GetComponentInParent<Pickup>().AddItems(oneHarvestTable.RollDrops(), out int count);
                if(count == 0) {
                    Notifications.CreateError(transform.position, d.noDrop);
                }
                GetComponentInParent<Pickup>().enabled = true;
                //still call if count is zero so it destroys if it's supposed to
                GetComponentInParent<Pickup>().Interact(player);
            }
        }
        public override void InitData(Data data) {
            base.InitData(data);
            gathering = false;
            remainingQuantity = quantity;
            currentGatherSteps = gatherSteps;
        }
    }
}