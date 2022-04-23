using UnityEngine;
using System.Collections;

namespace Itch {
    public class TravelBehavior : BehaviorTreeNode {
        public float restChance = .5f;
        public Vector2 travelDurationRange = new Vector2(20, 30);
        public Vector2 restDurationRange = new Vector2(20, 30);
        Vector2 travelDirection;
        public bool resting = false;
        public float endRestTime;
        [Range(0, 45)]
        public float angleDeviation;

        private void Start() {

            travelDirection = Random.insideUnitCircle.normalized;
        }

        public override bool Decide() {
            UpdateRest();
            return !resting;
        }

        private void OnEnable() {
            StartCoroutine(Travel());
        }

        IEnumerator Travel() {
            while(enabled) {
                if(Random.value < restChance) {
                    moveControl.motionDir = Vector2.zero;
                    endRestTime = Random.Range(restDurationRange.x, restDurationRange.y) + Time.time;
                    resting = true;
                } else {
                    resting = false;
                    float travelAngle = Vector2.SignedAngle(Vector2.right, travelDirection);
                    float newAngle = travelAngle + Random.Range(-angleDeviation, angleDeviation);
                    float radians = newAngle*Mathf.PI/180;
                    moveControl.motionDir = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
                }
                yield return new WaitForSeconds(Random.Range(travelDurationRange.x, travelDurationRange.y));
            }
        }

        public override void DoBehavior() {

        }

        void UpdateRest() {
            resting = Time.time < endRestTime;
        }

        public override bool CheckEnd() {
            UpdateRest();
            return resting;
        }
    }
}