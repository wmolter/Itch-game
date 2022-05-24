using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Itch {
    [System.Serializable]
    public class PoisonHealing : Behavior.IAttackEffect {
        public string tag;
        public float amount;
        public float interval;
        public float duration;
        public bool ignoreArmor;
        public void Attack(Health opponent, Entity self) {
            opponent.AddOverTime(this, self);
        }
    }

    public class Health : MonoBehaviour {
        public static bool Living(Component part) {
            Health targetHealth = part.GetComponent<Health>();
            if(targetHealth == null || targetHealth.enabled == false)
                return false;
            return targetHealth.Alive;
        }
        public struct EventData {
            public float current;
            public float percentage;
            public float change;
            public Entity byWho;
        }

        private class Overtime {
            public object tag;
            public float amount;
            public float interval;
            public float nextTime;
            public float endTime = Mathf.Infinity;
            public bool ignoreArmor;
            public Entity byWho;

            public static bool HasTag(Overtime o, object tag) {
                return o.tag == tag;
            }
        }

        public delegate void HealthEvent(EventData data);

        public float max;
        public float current;
        public float baseArmor = 0;
        public float armorBonus { get; set; }
        public UnityEvent OnDeath;
        public event HealthEvent OnDamaged;
        public event HealthEvent OnHealed;
        private List<Overtime> overtimes;
        private List<Overtime> Overtimes {
            get {
                if(overtimes == null)
                    overtimes = new List<Overtime>();
                return overtimes;
            } }

        public bool Alive { get { return current > 0; } }
        public float CurrentPercentage { get { return current/max; } }
        // Use this for initialization
        void Start() {
            //mostly for testing but..
            if(!Alive)
                Die();
        }

        // Update is called once per frame
        void Update() {
            if(Alive) {
                int i = 0;
                while(i < Overtimes.Count) {
                    Overtime o = Overtimes[i];
                    if(Time.time >= o.nextTime) {
                        SafeChange(o.amount, o.byWho, o.ignoreArmor);
                        o.nextTime = o.nextTime + o.interval;
                        if(o.nextTime > o.endTime) {
                            overtimes.RemoveAt(i);
                            i--;
                        }
                    }
                    i++;
                }
            }
        }

        public float GetPercentage() {
            return CurrentPercentage;
        }

        public float GetCurrent() {
            return current;
        }

        public float GetMax() {
            return max;
        }

        public System.IFormattable GetCurrentUI() {
            return current;
        }

        public System.IFormattable GetMaxUI() {
            return max;
        }

        public void SafeChange(float amount, Entity byWho, bool ignoreArmor) {
            if(amount > 0) {
                Heal(amount, byWho);
            } else {
                Damage(-amount, byWho, ignoreArmor);
            }
        }

        public void Heal(float amount, Entity byWho) {
            current = Mathf.Min(current + amount, max);
            Notifications.CreatePositive(transform.position, "+" + amount);
            OnHealed?.Invoke(new EventData { current = current, percentage = CurrentPercentage, change = amount, byWho = byWho });
        }

        public void Damage(float amount, Entity byWho) {
            Damage(amount, byWho, false);
        }

        public void Damage(float amount, Entity byWho, bool ignoreArmor) {
            float armor = ignoreArmor? 0 : armorBonus + baseArmor;
            float change = -(amount - armor);
            change = Mathf.Min(change, 0);
            current += change;
            Notifications.CreateNegative(transform.position, "" + -change);
            //Debug.Log("Damaged for: " + -change + " (" + amount + " - " + armor + " armor)" + " Current: " + current);
            OnDamaged?.Invoke(new EventData { current = current, percentage = CurrentPercentage, change = change, byWho = byWho });
            if(current <= 0)
                Die();
        }

        public void AddOverTime(float amount, float interval, object tag, Entity byWho) {
            AddOverTime(amount, interval, tag, byWho, true);
        }

        public void AddOverTime(float amount, float interval, object tag, Entity byWho, bool ignoreArmor) {
            AddOverTime(amount, interval, Mathf.Infinity, tag, byWho, ignoreArmor);
        }

        public void AddOverTime(PoisonHealing data, Entity byWho) {
            AddOverTime(data.amount, data.interval, data.duration, data.tag, byWho, data.ignoreArmor);
        }

        public void AddOverTime(float amount, float interval, float duration, object tag, Entity byWho, bool ignoreArmor) {
            Overtime prev = Overtimes.Find(delegate (Overtime o) { return Overtime.HasTag(o, tag); });
            if(prev == null) {
                Overtimes.Add(new Overtime() { amount=amount, interval=interval, tag=tag, byWho = byWho, nextTime=Time.time + interval, endTime = duration+Time.time });
            } else {
                prev.interval = interval;
                prev.amount = amount;
            }
        }

        public void RemoveOverTime(object tag) {
            int toRemove = Overtimes.FindIndex(delegate (Overtime o) { return Overtime.HasTag(o, tag); });
            if(toRemove >= 0)
                Overtimes.RemoveAt(toRemove);
        }

        public void Die() {
            Debug.Log(name + " died.");
            OnDeath?.Invoke();
        }
    }
}