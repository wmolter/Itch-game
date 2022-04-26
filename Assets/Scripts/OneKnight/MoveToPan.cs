using UnityEngine;
using System.Collections.Generic;


namespace OneKnight.Cameras {
    public class MoveToPan : MonoBehaviour {
        
        public List<Transform> toMove;

        public void Move(float x, float z) {
            Move(new Vector3(x, 0, z));
        }

        public void Move(Vector3 moveVec) {
            for(int i = 0; i < toMove.Count; i++) {
                toMove[i].position += moveVec;
            }
        }

        public void RotateAround(Vector3 position, Vector3 axis, float angle) {
            for(int i = 0; i < toMove.Count; i++) {
                toMove[i].RotateAround(position, axis, angle);
            }
        }
    }
}