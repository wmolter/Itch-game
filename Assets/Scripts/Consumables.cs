using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch {
    public static class Consumables {
        public const string Category = "Consumable";
        public const string BuffName = "buffName";
        public const string BuffStrength = "buffStrength";
        public const string BuffDuration = "buffDuration";
        
        public static InventoryItem Use(InventoryItem consumable) {
            PlayerManager p = PlayerManager.instance;
            p.GiveHealth(consumable.Value);
            if(consumable.HasProperty(BuffName)) {
                string name = consumable.GetProperty<string>(BuffName);
                float strength = consumable.GetProperty<float>(BuffStrength);
                float duration = consumable.GetProperty<float>(BuffDuration);
                p.GiveBuff(name, strength, duration);
            }
            return consumable.RemoveOne();
        }

        public static bool TryUse(ItemSlot slot) {
            InventoryItem i = slot.Item;
            if(!slot.Empty && i.Category == Category) {
                Use(slot.RemoveOne());
                return true;
            }
            return false;
        }
    }
}