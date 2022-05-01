using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

namespace Itch {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Entity : MonoBehaviour {

        public float sortingOffset;
        [HideInInspector]
        public Vector2 home;


        public Vector3Int CurrentTile {
            get {
                return new Vector3Int((int)(transform.position.x), (int)(transform.position.y), (int)transform.position.z);
            }
        }

        public int CurrentTileType {
            get {
                TileBase tile = Planes.CurrentPlane.map.GetTile(CurrentTile);
                int i;
                for(i = Planes.CurrentPlane.allTerrainData.Length-1; i >= 0 && Planes.CurrentPlane.allTerrainData[i].tile != tile; i--) {

                }
                return i;
            }
        }
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void LateUpdate() {
            GetComponent<SpriteRenderer>().sortingOrder = (int)(-32*(transform.position.y+sortingOffset));
        }
    }
}