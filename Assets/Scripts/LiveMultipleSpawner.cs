using UnityEngine;
using System.Collections;

namespace Itch {
    public class LiveMultipleSpawner : EntitySpawner {

        public float waitTime;
        public bool multiple;
        public bool destroyObject;
        // Use this for initialization

        protected override void Start() {
        }

        protected virtual void OnEnable() {
            StartCoroutine(DelaySpawn());
        }

        IEnumerator DelaySpawn() {
            yield return new WaitForSeconds(waitTime);
            Spawn();
            enabled = false;
            if(!multiple) {
                if(destroyObject)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
        }
        // Update is called once per frame
        void Update() {

        }
    }
}