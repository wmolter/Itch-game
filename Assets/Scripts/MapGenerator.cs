using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using OneKnight;

namespace Itch {
    public class MapGenerator : MonoBehaviour {
        public Tilemap map;
        public TerrainData[] allTerrainData;
        //public List<NoiseLayer> genLayers;
        public NoiseParams settings;
        public bool voronoiMode;
        public float vorFreq;
        public NoiseParams noise2;
        public float weight;
        public float offset = .5f;
        public PlayerManager player;
        public Vector3 noiseOffset;
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
            if(voronoiMode)
                value = Noise.VoronoiCellLayer(position, vorFreq, settings);//Noise.LayerNoiseSample(position, genLayers);
            else
                value = Noise.Sum(position, settings);
            value += weight*Noise.Sum(position, noise2) + offset;
            return Mathf.Clamp(Mathf.FloorToInt(value*allTerrainData.Length), 0, allTerrainData.Length - 1);
        }

        void UpdateTiles() {
            Vector3 playerPos = player.transform.position;
            Vector3Int playerTile = player.CurrentTile;
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
            int currIndex = TileIndex(player.CurrentTile);
            for(int i = 0; i < allTerrainData.Length; i++) {
                if(allTerrainData[i].effect != null)
                    allTerrainData[i].effect.enabled = i==currIndex;
            }
        }

        [System.Serializable]
        public struct Spawn {
            public float weight;
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

        GameObject SpawnObject(Spawn objData, Vector3 position) {
            return SpawnObject(objData.type, position);
        }

        GameObject SpawnObject(string type, Vector3 position) {
            GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/" + type), map.transform);
            newObj.transform.position = position + (Vector3)(Vector2.one*.5f);
            return newObj;
        }

        public void PlaceItem(InventoryItem item, Vector3 position, float searchRadius) {
            Gatherable onGround = FindSpawnBackpack(position, searchRadius);
            onGround.AddHarvestedResource(item);
        }

        public void PlaceItems(List<InventoryItem> items, Vector3 position, float searchRadius) {
            Gatherable onGround = FindSpawnBackpack(position, searchRadius);
            foreach(InventoryItem item in items) {
                onGround.AddHarvestedResource(item);
            }
        }

        private Gatherable FindSpawnBackpack(Vector3 position, float searchRadius) {//search default layer
            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(position, searchRadius, 1);
            Gatherable onGround = null;
            for(int i = 0; i < nearbyObjects.Length; i++) {
                if(nearbyObjects[i].tag == "drops")
                    onGround = nearbyObjects[i].GetComponent<Gatherable>();
            }
            if(onGround == null)
                onGround = SpawnObject("Backpack", position).GetComponent<Gatherable>();
            return onGround;
        }
    }
}