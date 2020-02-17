using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch {
    [RequireComponent(typeof(Interactable), typeof(SpriteRenderer))]
    public class CraftingNeeded : MonoBehaviour {

        public Drop[] materialsNeeded;
        public float craftTime;
        public MonoBehaviour unlockedComponent;
        public Sprite craftedSprite;
        float finishTime;
        // Use this for initialization
        void Awake() {
            GetComponent<Interactable>().Interaction = TryCraft;
        }

        // Update is called once per frame
        void Update() {

        }

        void TryCraft(PlayerManager player) {
            if(player.CheckInventoryHas(materialsNeeded))
                StartCoroutine(Craft(player));
            else {
                Notifications.CreateError(transform.position, "Not enough materials");
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
                if(player.CheckConsumeFromInventory(materialsNeeded)) {
                    CraftFinished();
                } else {
                    Notifications.CreateError(transform.position, "Materials missing");
                }
            }
            hint.Dismiss();
        }

        void CraftFinished() {

            unlockedComponent.enabled = true;
            GetComponent<SpriteRenderer>().sprite = craftedSprite;
            Destroy(this);
        }
    }
}