using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public class EntitySpawner : MonoBehaviour {

        [System.Serializable]
        public struct EntitySpawn {
            public int quantity;
            public string prefabName;
        }

        [System.Serializable]
        public enum Mode {
            Random, UniformRadial
        }

        public List<EntitySpawn> spawns;
        public Mode mode = Mode.Random;
        public float spawnRadius;
        public float exclusionRadius;

        public float adjustmentRadius;
        public string prefix = "Enemies/";
        List<Entity> spawned;
        // Use this for initialization
        private void Awake() {
            spawned = new List<Entity>();
        }

        void Start() {
            switch(mode) {
                case Mode.Random:
                    foreach(EntitySpawn spawn in spawns) {
                        for(int i = 0; i < spawn.quantity; i++) {
                            float angle = Random.Range(0, 2*Mathf.PI);
                            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            InitEntity(spawn, dir*Random.Range(exclusionRadius, spawnRadius));
                        }
                    }
                    break;

                case Mode.UniformRadial:
                    List<Vector2> pos = new List<Vector2>();
                    int spawnQuantity = 0;
                    foreach(EntitySpawn spawn in spawns)
                        spawnQuantity += spawn.quantity;
                    for(int i = 0; i < spawnQuantity; i++) {
                        float angle = i*2*Mathf.PI/spawnQuantity;
                        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        Vector2 newPos = dir* Random.Range(exclusionRadius, spawnRadius);
                        newPos += Random.insideUnitCircle*adjustmentRadius;
                        pos.Add(newPos);
                    }
                    foreach(EntitySpawn spawn in spawns) {
                        for(int i = 0; i < spawn.quantity; i++) {
                            int index = Random.Range(0, pos.Count);
                            InitEntity(spawn, pos[index]);
                            pos.RemoveAt(index);
                        }
                    }
                    break;
                
            }
        }

        void InitEntity(EntitySpawn spawn, Vector2 position) {
            Entity newEntity = Instantiate(Resources.Load<Entity>("Prefabs/" + prefix + spawn.prefabName), transform);
            newEntity.transform.localPosition = position;
            spawned.Add(newEntity);
            newEntity.home = transform.position;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}