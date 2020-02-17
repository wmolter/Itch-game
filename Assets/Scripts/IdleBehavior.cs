using UnityEngine;
using System.Collections;

namespace Itch {
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
                    mainControl.motionDir = Vector2.zero;
                } else if(mainControl.motionDir == Vector2.zero){
                    mainControl.motionDir = Random.insideUnitCircle.normalized;
                } else {
                    float currAngle = Vector2.SignedAngle(Vector2.right, mainControl.motionDir);
                    float newAngle = currAngle + Random.Range(-angleChangeRange, angleChangeRange);
                    float radians = newAngle*Mathf.PI/180;
                    mainControl.motionDir = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
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