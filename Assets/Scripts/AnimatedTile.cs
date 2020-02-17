using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

namespace Itch {
    [CreateAssetMenu]
    public class AnimatedTile : TileBase {
        public Sprite[] m_AnimatedSprites;
        public Tile.ColliderType colliderType;
        public float m_AnimationSpeed = 1f;
        public float m_AnimationStartTime;

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            tileData.colliderType = colliderType;
            if(m_AnimatedSprites != null && m_AnimatedSprites.Length > 0) {
                tileData.sprite = m_AnimatedSprites[m_AnimatedSprites.Length - 1];
            }
        }

        public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData) {
            
            if(m_AnimatedSprites != null && m_AnimatedSprites.Length > 0) {
                tileAnimationData.animatedSprites = m_AnimatedSprites;
                tileAnimationData.animationSpeed = m_AnimationSpeed;
                tileAnimationData.animationStartTime = m_AnimationStartTime;
                return true;
            }
            return false;
        }
    }
}