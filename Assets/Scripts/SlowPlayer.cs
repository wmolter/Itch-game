using UnityEngine;
using System.Collections;

namespace Itch {
    public class SlowPlayer : MonoBehaviour {
        public float percentage = .5f;
        public int id = 100;
        // Use this for initialization
        void Start() {

        }

        private void OnEnable() {
            PlayerManager.instance.properties.AddModifier("Speed", percentage, id);
        }

        private void OnDisable() {
            PlayerManager.instance.properties.RemoveAdjustment("Speed", id);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}