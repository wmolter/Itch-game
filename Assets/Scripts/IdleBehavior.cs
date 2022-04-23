using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Movement))]
    public class IdleBehavior : BehaviorTreeNode {
        public float standstillChance = .5f;
        public Vector2 durationRange = new Vector2(2, 5);
        public float angleChangeRange = 90;

        public override bool Decide() {
            return true;
        }

        private void OnEnable() {
            StartCoroutine(IdleMovement());
        }

        IEnumerator IdleMovement() {
            while(enabled) {
                if(Random.value < standstillChance) {
                    moveControl.motionDir = Vector2.zero;
                } else if(moveControl.motionDir == Vector2.zero){
                    moveControl.motionDir = Random.insideUnitCircle.normalized;
                } else {
                    float currAngle = Vector2.SignedAngle(Vector2.right, moveControl.motionDir);
                    float newAngle = currAngle + Random.Range(-angleChangeRange, angleChangeRange);
                    float radians = newAngle*Mathf.PI/180;
                    Vector2 newDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                    //Debug.Log(name + " Old angle: " + currAngle + " New angle: " + newAngle + " Old motion dir: " + moveControl.motionDir + " New motion dir: " + newDir);

                    moveControl.motionDir = newDir;
                }
                yield return new WaitForSeconds(Random.Range(durationRange.x, durationRange.y));
            }
        }

        public override void DoBehavior() {

        }

        public override bool CheckEnd() {
            return false;
        }
        
    }
}