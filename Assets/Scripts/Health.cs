using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Itch {
    public class Health : MonoBehaviour {

        public float max;
        public float current;
        public UnityEvent OnDeath;

        public float CurrentPercentage { get { return current/max; } }
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

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

        public void Heal(float amount) {
            current = Mathf.Min(current + amount, max);
            Notifications.CreatePositive(transform.position, "+" + amount);
        }

        public void Damage(float amount) {
            current -= amount;
            Notifications.CreateNegative(transform.position, "" + amount);
            Debug.Log("Damaged for: " + amount + ".  Current: " + current);
            if(current <= 0)
                Die();
        }

        public void Die() {
            Debug.Log(name + " died.");
            OnDeath?.Invoke();
        }
    }
}