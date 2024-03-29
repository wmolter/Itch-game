﻿using UnityEngine;
using System.Collections.Generic;
using OneKnight;

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
        public bool useParent = false;
        List<Entity> spawned;
        // Use this for initialization
        private void Awake() {
            spawned = new List<Entity>();
        }

        protected virtual void Start() {
            Debug.Log("Started spawner for " + gameObject.name);
            Spawn();
        }

        //only public so disabled scripts can have this called during death through event systems
        public void Spawn() {

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
                    pos.AddRange(Utils.UniformRadialPositions(spawnQuantity, Mathf.PI*2, exclusionRadius, spawnRadius, adjustmentRadius));
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
            Transform toParent = useParent ? transform.parent: transform;
            Entity newEntity = Instantiate(Resources.Load<Entity>("Prefabs/" + prefix + spawn.prefabName), toParent);
            newEntity.transform.localPosition = position;
            if(useParent)
                newEntity.transform.localPosition += transform.localPosition;
            spawned.Add(newEntity);
            newEntity.home = transform.position;
            //again, not the best, but should be ok
            newEntity.homePlane = Planes.CurrentPlaneIndex;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}