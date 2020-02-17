using UnityEngine;
using System.Collections;
namespace Itch {
    public class AvoidCollisionBehavior : BehaviorTreeNode {

        bool shouldActivate = false;
        float tryAngle;
        Vector2 oldMotionDir;

        public Vector2 angleFromBackwardRange;


        public override bool Decide() {
            return shouldActivate;
        }

        private void OnEnable() {
            StartCoroutine(EvasiveManeuvers());
        }

        IEnumerator EvasiveManeuvers() {
            oldMotionDir = mainControl.motionDir;
            while(enabled) {
                yield return null;
            }
        }

        public override void DoBehavior() {

        }

        public override bool CheckEnd() {
            return false;
        }

        public override void OnFinish() {
            mainControl.motionDir = oldMotionDir;
        }

        public float stuckDurationToActivate = 1f;
        float startTime;
        private void OnCollisionEnter2D(Collision2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Default")) {
                startTime = Time.time;
                float angle = Vector2.SignedAngle(collision.relativeVelocity, Vector2.right);
                angle += Random.Range(angleFromBackwardRange.x, angleFromBackwardRange.y);
                tryAngle = angle;
            }
        }

        private void Update() {
           // if()
        }
    }
}