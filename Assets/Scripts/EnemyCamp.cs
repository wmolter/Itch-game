using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public class EnemyCamp : MonoBehaviour {

        [System.Serializable]
        public struct EnemySpawn {
            public int quantity;
            public string prefabName;
        }

        public List<EnemySpawn> spawns;
        public float spawnRadius;
        List<Enemy> spawned;
        // Use this for initialization
        private void Awake() {
            spawned = new List<Enemy>();
        }

        void Start() {
            foreach(EnemySpawn spawn in spawns) {
                for(int i = 0; i < spawn.quantity; i++) {
                    Enemy newEnemy = Instantiate(Resources.Load<Enemy>("Prefabs/Enemies/" + spawn.prefabName), transform);
                    newEnemy.transform.localPosition = spawnRadius*Random.insideUnitCircle;
                    spawned.Add(newEnemy);
                    newEnemy.home = transform.position;
                }
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}