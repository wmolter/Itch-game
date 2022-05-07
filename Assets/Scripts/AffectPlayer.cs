using UnityEngine;
using System.Collections;

namespace Itch {
    public class AffectPlayer : MonoBehaviour {
        public float percentage = .5f;
        public string id = "slow";
        public string property = "Movement";
        // Use this for initialization
        void Start() {

        }

        private void OnEnable() {
            PlayerManager.instance.properties.AddModifier(property, percentage, id);
        }

        private void OnDisable() {
            PlayerManager.instance.properties.RemoveAdjustment(property, id);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}