using UnityEngine;
using System.Collections;

namespace OneKnight.InventoryManagement {
    [System.Serializable]
    public class VolumeSlot : FilterItemSlot {

        [System.NonSerialized]
        private VolumeInventory parent;

        [System.NonSerialized]
        float prevVolume;

        public VolumeSlot(string[] categories, VolumeInventory parent) : base(categories) {
            this.parent = parent;
            OnChange += DoOnChange;
            BeforeChange += DoBeforeChange;
        }

        public override bool PutItem(InventoryItem toAdd) {
            float addedVolume = toAdd == null? 0 : toAdd.Volume;
            if(parent.CheckVolumeChange(addedVolume, this)) {
                bool result = base.PutItem(toAdd);
                return result;
            }
            return false;
        }

        public override InventoryItem Stack(InventoryItem other, int max) {
            float addedVolume = 0;
            if(other != null) {
                if(Empty) {
                    addedVolume = Mathf.Min(other.Volume, max*other.VolumePer);
                } else {
                    //maximum volume - current volume
                    addedVolume = Mathf.Min(other.Volume, max*other.VolumePer, other.VolumePer*other.StackLimit - Volume);
                }

                if(parent.CheckVolumeChange(addedVolume, this)) {
                    return base.Stack(other, max);
                } else {
                    int fitsCount = Mathf.FloorToInt(parent.RemainingVolume/other.VolumePer);
                    if(fitsCount > 0)
                        return base.Stack(other, fitsCount);
                    else
                        return other;
                }
            } else {
                return null;
            }
        }

        private void DoBeforeChange(ItemSlot slot) {
            prevVolume = Volume;
            parent.NeedsVolumeUpdate = true;
        }

        private void DoOnChange(ItemSlot slot) {
            parent.ChangeVolume(Volume - prevVolume);
            parent.TrimExpand();
        }

        public void SetParent(VolumeInventory parent) {
            this.parent = parent;
        }
    }
}