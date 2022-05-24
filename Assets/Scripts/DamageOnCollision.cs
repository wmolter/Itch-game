using UnityEngine;
using System.Collections.Generic;
using Itch.Behavior;
using UnityEngine.Events;

namespace Itch {
    [RequireComponent(typeof(Health))]
    public class DamageOnCollision : MonoBehaviour {

        public float damage;
        public float interval;
        public AOEHandler filter;
        

        private void OnTriggerEnter2D(Collider2D collider) {
            if(filter.FilterWithLayers(collider))
                GetComponent<Health>().AddOverTime(-damage, interval, collider, collider.GetComponent<Entity>());
        }

        private void OnTriggerExit2D(Collider2D collider) {
            if(filter.FilterWithLayers(collider))
                GetComponent<Health>().RemoveOverTime(collider);
        }
    }
}