using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using OneKnight.Generation;
using OneKnight;

namespace Itch {
    public class MapGenerator : MonoBehaviour {
        public Tilemap map;
        public TerrainData[] allTerrainData;
        //public List<NoiseLayer> genLayers;
        public NoiseParams settings;
        public bool voronoiMode;
        public float vorFreq;
        public NoiseParams preNoise;
        public float preWeight;
        public float offset = .5f;
        public PlayerManager player;
        public Vector3 noiseOffset;
        public Vector3 prenoiseOffsetA;
        public Vector3 prenoiseOffsetB;
        public float SightModifier = 1;
        public float gravity = 5;
        int xpPerTile = 1;
        // Start is called before the first frame update
        void Start() {
            foreach(TerrainData data in allTerrainData) {
                float totalWeight = 0;
                foreach(Spawn possible in data.spawns) {
                    totalWeight += possible.weight;
                }
                data.totalWeight = totalWeight;
            }
            UpdateTiles();
        }

        // Update is called once per frame
        void Update() {
            UpdateTiles();
            UpdateEffect();
        }

        int TileIndex(Vector3Int position) {
            float value;
            float x = preWeight*Noise.Sum(position + prenoiseOffsetA, preNoise);
            float y = preWeight*Noise.Sum(position + prenoiseOffsetB, preNoise);
            Vector3 pos = position + new Vector3(x, y, 0);
            //should be between 0 and 1, if we can, to make distribution of biomes..
            value = .5f * Noise.Sum(pos, settings) + offset;
            /*
            if(voronoiMode)
                value = Noise.VoronoiCellLayer(position, vorFreq, settings);//Noise.LayerNoiseSample(position, genLayers);
            else
                value = Noise.Sum(position, settings);
            value += weight*Noise.Sum(position, preNoise) + offset;*/
            return Mathf.Clamp(Mathf.FloorToInt(value*allTerrainData.Length), 0, allTerrainData.Length - 1);
        }

        void UpdateTiles() {
            Vector3 playerPos = player.transform.position;
            Vector3Int playerTile = player.GetComponent<Entity>().CurrentTile;
            int chunkSize = Mathf.CeilToInt(2*player.LOS)+1;
            int count = 0;
            for(int i = -1; i < chunkSize; i++) {
                for(int j = -1; j < chunkSize; j++) {
                    Vector3Int position = playerTile + new Vector3Int(i - chunkSize/2, j - chunkSize/2, 0);
                    if(map.GetTile(position) == null && (position - playerPos + new Vector3(.5f, .5f, 0)).magnitude < player.LOS) {
                        int index = TileIndex(position);
                        map.SetTile(position, allTerrainData[index].tile);
                        count++;

                        CheckGenerateObject(position, index);
                    }
                }
            }
            player.NewTiles(count, xpPerTile);
        }

        void UpdateEffect() {
            int currIndex = player.GetComponent<Entity>().CurrentTileType;
            for(int i = 0; i < allTerrainData.Length; i++) {
                if(allTerrainData[i].effect != null)
                    allTerrainData[i].effect.enabled = i==currIndex;
            }
        }

        [System.Serializable]
        public struct Spawn : Weighted{
            public float weight;
            public float Weight { get { return weight; } }
            public string type;
        }

        [System.Serializable]
        public class TerrainData {
            public TileBase tile;
            public float objectChance;
            public List<Spawn> spawns;
            public MonoBehaviour effect;
            [HideInInspector]
            public float totalWeight;
        }

        void CheckGenerateObject(Vector3Int position, int terrainIndex) {
            Vector3 useForNoise = new Vector3(position.x, position.y, 0)+noiseOffset;
            float value = Noise.WhiteNoise3D(useForNoise);
            TerrainData s = allTerrainData[terrainIndex];
            if(value < s.objectChance) {
                //Debug.Log("Noise value: " + value + " with position: " + position + ", noise position: " + useForNoise + " and terrain: " + terrainIndex);
                value /= s.objectChance;
                value *= s.totalWeight;
                //Debug.Log("Generation value: " + value);
                float weight = 0;
                foreach(Spawn possible in s.spawns) {
                    weight += possible.weight;
                    if(value < weight) {
                        SpawnObject(possible, position);
                        break;
                    }
                }
            }
            /*
            if(value > .99f) {
                if(terrainIndex == 3)
                    newObj = Instantiate(Resources.Load<GameObject>("Prefabs/AetherPortal"), map.transform);
                else
                    newObj = Instantiate(Resources.Load<GameObject>("Prefabs/Cavern"), map.transform);
            } else if(value > .9f) {
                if(terrainIndex < 2) {
                    newObj = Instantiate(Resources.Load<GameObject>("Prefabs/Tuft"), map.transform);
                } else {
                    newObj = Instantiate(Resources.Load<GameObject>("Prefabs/Rock"), map.transform);
                }
            }
            if(newObj != null) {

                newObj.transform.position = position + (Vector3)(Vector2.one*.5f);
            }*/
        }

        Entity SpawnObject(Spawn objData, Vector3 position) {
            return SpawnObject(objData.type, position);
        }

        public Entity SpawnObject(string type, Vector3 position) {
            Entity newObj = Instantiate(Resources.Load<Entity>("Prefabs/" + type), map.transform);
            newObj.transform.position = position + (Vector3)(Vector2.one*.5f);
            newObj.home = newObj.transform.position;
            //not great, because a map generator is a plane, and maybe it should know what itself is, and what if we generate things when it's not active? but should be okay for now idk
            newObj.homePlane = Planes.CurrentPlaneIndex;
            return newObj;
        }

        public void PlaceItem(InventoryItem item, Vector3 position, float searchRadius) {
            Pickup onGround = FindSpawnBackpack(position, searchRadius);
            onGround.AddItem(item);
        }

        public void PlaceItems(List<InventoryItem> items, Vector3 position, float searchRadius) {
            Pickup onGround = FindSpawnBackpack(position, searchRadius);
            onGround.AddItems(items);
        }

        private Pickup FindSpawnBackpack(Vector3 position, float searchRadius) {//search default layer
            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(position, searchRadius, 1);
            Pickup onGround = null;
            for(int i = 0; i < nearbyObjects.Length; i++) {
                if(nearbyObjects[i].tag == "drops")
                    onGround = nearbyObjects[i].GetComponent<Pickup>();
            }
            if(onGround == null)
                onGround = SpawnObject("Backpack", position).GetComponent<Pickup>();
            return onGround;
        }
    }
}