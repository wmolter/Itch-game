using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch {
    [RequireComponent(typeof(SpriteRenderer))]
    public class CraftingNeeded : LevelRequiredAction {

        public Drop[] materialsNeeded;
        public float craftTime;
        public MonoBehaviour unlockedComponent;
        public Sprite craftedSprite;
        float finishTime;
        // Use this for initialization
        void Awake() {
        }

        // Update is called once per frame
        void Update() {

        }

        protected override void DoAction(PlayerManager player) {
            Drop[] lacking;
            if(player.CheckInventoryHas(materialsNeeded, out lacking))
                StartCoroutine(Craft(player));
            else {
                Notifications.CreateError(transform.position, "Materials missing: " + Strings.ItemList(lacking));
            }
        }

        IEnumerator Craft(PlayerManager player) {
            ProgressHint hint = Notifications.CreateHint(transform.position);
            finishTime = Time.time + craftTime;
            while(Time.time < finishTime && player.interacting) {
                hint.SetProgress((finishTime-Time.time)/craftTime);
                yield return null;
            }
            if(player.interacting) {
                Drop[] lacking;
                if(player.CheckConsumeFromInventory(materialsNeeded, out lacking)) {
                    CraftFinished();
                } else {
                    Notifications.CreateError(transform.position, "Materials missing: " + Strings.ItemList(lacking));
                }
            }
            hint.Dismiss();
        }

        void CraftFinished() {

            unlockedComponent.enabled = true;
            if(craftedSprite != null)
                GetComponent<SpriteRenderer>().sprite = craftedSprite;
            enabled = false;
        }
    }
}