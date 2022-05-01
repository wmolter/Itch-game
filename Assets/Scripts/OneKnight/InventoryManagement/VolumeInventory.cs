using UnityEngine;
using System.Collections;
using OneKnight.PropertyManagement;

namespace OneKnight.InventoryManagement {
    [System.Serializable]
    public class VolumeInventory : FilterInventory {

        public float Volume { get; private set; }

        public float TrueUsedVolume { get; private set; }

        public float UsedVolume {
            get {
                    return TrueUsedVolume;
            }
        }

        public float RemainingVolume {
            get {
                return Volume - UsedVolume;
            }
        }

        public bool Valid {
            get {
                return UsedVolume <= Volume;
            }
        }

        public bool NeedsVolumeUpdate = false;

        private VolumeSlot largest;
        private float largestVolume;

        public VolumeInventory(float volume) : this(volume, null) {
        }

        public VolumeInventory(float volume, string[] categories) : this(volume, categories, null) {
        }

        public VolumeInventory(float volume, string[] categories, PropertyManager properties) : base(categories, 2, properties) {
            Volume = volume;
            TrimExpand();
            CalculateVolume();
        }
        
        public void InitAfterLoad() {
            FixCapacity(true);
            foreach(ItemSlot slot in this) {
                ((VolumeSlot)slot).SetParent(this);
            }
        }

        public void SetMaxVolume(float volume) {
            SetMaxVolume(volume, true);
        }

        public void SetMaxVolume(float volume, bool safe) {
            volume = AdjustProperty("storageVolume", volume);
            if(safe && volume < UsedVolume)
                return;
            Volume = volume;
        }

        protected override ItemSlot FindEmpty() {
            ItemSlot slot = base.FindEmpty();
            if(slot == null)
                Capacity++;
            ValidateCapacity();
            return base.FindEmpty();
        }

        public virtual void TrimExpand() {
            int i = Capacity-1;
            while(i > 0 && GetSlot(i).Empty)
                i--;
            Capacity = i+2; //leave one empty slot
            ValidateCapacity();
        }

        public void CalculateVolume() {
            TrueUsedVolume = 0;
            largestVolume = 0;
            foreach(ItemSlot slot in this) {
                TrueUsedVolume += slot.Volume;
                if(slot.Volume >= largestVolume) {
                    largest = (VolumeSlot)slot;
                    largestVolume = slot.Volume;
                }
            }
            NeedsVolumeUpdate = false;
        }

        public void FixCapacity(bool stack) {
            Compress(stack);
            TrimExpand();
            CalculateVolume();
        }

        public bool CheckVolumeChange(float change, ItemSlot slot) {
            return UsedVolume + change <= Volume;
        }

        public void ChangeVolume(float change) {
            if(NeedsVolumeUpdate)
                TrueUsedVolume += change;
            NeedsVolumeUpdate = false;
        }


        protected override void OnItemChanged(ItemSlot.EventInfo changed) {
            base.OnItemChanged(changed);
            if(changed.slot == largest && changed.slot.Volume < largestVolume) {
                CalculateVolume();
            } else {
                if(changed.slot.Volume > largest.Volume) {
                    largest = (VolumeSlot)changed.slot;
                    largestVolume = changed.slot.Volume;

                }
            }
        }

        protected override ItemSlot CreateSlot(){
            VolumeSlot slot = new VolumeSlot(categories, this);
            slot.OnChange += OnItemChanged;
            return slot;
        }
    }
}