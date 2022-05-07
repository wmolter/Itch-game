using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Itch {
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
            public string tag;
            public float amount;
            public float interval;
            public float nextTime;
            public Entity byWho;

            public static bool HasTag(Overtime o, string tag) {
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
                foreach(Overtime o in Overtimes) {
                    if(Time.time >= o.nextTime) {
                        SafeChange(o.amount, o.byWho);
                        o.nextTime = o.nextTime + o.interval;
                    }
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

        public void SafeChange(float amount, Entity byWho) {
            if(amount > 0) {
                Heal(amount, byWho);
            } else {
                Damage(amount, byWho);
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
            current += change;
            Notifications.CreateNegative(transform.position, "" + amount);
            Debug.Log("Damaged for: " + amount + ".  Current: " + current);
            OnDamaged?.Invoke(new EventData { current = current, percentage = CurrentPercentage, change = change, byWho = byWho });
            if(current <= 0)
                Die();
        }

        public void AddRegen(float amount, float interval, string tag, Entity byWho) {
            Overtime prev = Overtimes.Find(delegate (Overtime o) { return Overtime.HasTag(o, tag); });
            if(prev == null) {
                Overtimes.Add(new Overtime() { amount=amount, interval=interval, tag=tag, byWho = byWho, nextTime=Time.time + interval });
            } else {
                prev.interval = interval;
                prev.amount = amount;
            }
        }

        public void RemoveRegen(string tag) {
            int toRemove = Overtimes.FindIndex(delegate (Overtime o) { return Overtime.HasTag(o, tag); });
            Overtimes.RemoveAt(toRemove);
        }

        public void Die() {
            Debug.Log(name + " died.");
            OnDeath?.Invoke();
        }
    }
}