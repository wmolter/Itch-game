using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    [CreateAssetMenu(menuName = "Generation/CraftGatherData")]
    public class CraftGatherData : LevelRequiredAction.Data {

        public InventoryItem[] materialsNeeded;
        public int quantity = 1;
        public int gatherSteps = 4;
        public float stepDuration = .25f;

        public DropManager oneHarvestTable;

        public string buffName = "";
        public float buffStrength = 1;
        public float buffDuration = 10;

        public string unlockedComponentName;
        public Sprite finishedSprite;
        public string finishedAnimationState;
        public bool disableOnFinish;
        public Vector2 hintOffset;

        [Header("strings")]
        public string alreadyHarvested = "No resources left.";
    }
}