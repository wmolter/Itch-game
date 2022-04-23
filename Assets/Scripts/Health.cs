using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Itch {
    public class Health : MonoBehaviour {

        public float max;
        public float current;
        public UnityEvent OnDeath;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void Damage(float amount) {
            current -= amount;
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