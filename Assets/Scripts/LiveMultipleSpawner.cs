using UnityEngine;
using System.Collections;

namespace Itch {
    public class LiveMultipleSpawner : EntitySpawner {

        public float waitTime;
        public bool multiple;
        public int maxChildren;
        public bool destroyObject;
        private bool waiting = false;
        // Use this for initialization

        protected override void Start() {
        }

        protected virtual void OnEnable() {
        }

        IEnumerator DelaySpawn() {
            waiting = true;
            yield return new WaitForSeconds(waitTime);
            if(transform.childCount < maxChildren)
                Spawn();
            if(!multiple) {
                if(destroyObject)
                    Destroy(gameObject);
                else
                    enabled = false;
            }
            waiting = false;
        }
        // Update is called once per frame
        void Update() {
            if(!waiting)
                StartCoroutine(DelaySpawn());
        }
    }
}