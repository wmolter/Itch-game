using UnityEngine;
using System.Collections;
using OneKnight;
using Itch.Effects;

namespace Itch {
    public static class Consumables {
        public const string Category = "Consumable";
        public const string BuffName = "effectName";
        public const string BuffStrength = "effectStrength";
        public const string BuffDuration = "effectDuration";
        public const string SummonPrefab = "summonPrefab";
        
        public static InventoryItem Use(InventoryItem consumable) {
            PlayerManager p = PlayerManager.instance;
            if(consumable.Value >= 0)
                p.GiveHealth(consumable.Value);
            else 
                p.Damage(-consumable.Value);

            //Debug.Log("Consuming..." + consumable.GetBaseProperty<object>(BuffName));
            if(consumable.HasProperty<string>(BuffName)) {
                string name = consumable.GetBaseProperty<string>(BuffName);
                Effect toUse = FindEffect(name);
                if(toUse == null) {
                    float strength = consumable.GetFloatProperty(BuffStrength);
                    float duration = consumable.GetFloatProperty(BuffDuration);
                    toUse = new ChangeLevel() { effectName = name, positive = strength >= 0, strength = Mathf.Abs(strength), duration = duration };
                    p.StartEffect(toUse, p.transform.position);
                } else if(consumable.HasProperty<float>(BuffDuration)) {
                    p.StartEffect(toUse, consumable.GetFloatProperty(BuffDuration), p.transform.position);
                } else {
                    p.StartEffect(toUse, p.transform.position);
                }
            }
            if(consumable.HasProperty<string[]>(BuffName)) {
                //Debug.Log("Array of effects.");
                string[] names = consumable.GetBaseProperty<string[]>(BuffName);
                float[] strengths = consumable.GetBaseProperty<float[]>(BuffStrength);
                float[] durations = consumable.GetBaseProperty<float[]>(BuffDuration);
                for(int i = 0; i < names.Length; i++) {
                    Effect toUse = FindEffect(names[i]);
                    if(toUse == null) {
                        float strength = strengths[i];
                        float duration = durations[i];
                        toUse = new ChangeLevel() { effectName = names[i], positive = strength >= 0, strength = Mathf.Abs(strength), duration = duration };
                        p.StartEffect(toUse, p.transform.position);
                    } else if(durations != null) {
                        p.StartEffect(toUse, durations[i], p.transform.position);
                    } else {
                        p.StartEffect(toUse, p.transform.position);
                    }
                }
            }
            if(consumable.HasProperty(SummonPrefab)) {
                Entity summoned = Planes.CurrentPlane.SpawnObject(consumable.GetBaseProperty<string>(SummonPrefab), p.transform.position);
                Tamed tamed = summoned.GetComponent<Tamed>();
                if(tamed != null) {
                    tamed.enabled = true;
                    p.GetComponent<FollowerManager>().ToggleFollower(tamed);
                }
            }
            return consumable.RemoveOne();
        }

        static Effect FindEffect(string name) {
            return Resources.Load<Effect>("Effects/" + name);
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