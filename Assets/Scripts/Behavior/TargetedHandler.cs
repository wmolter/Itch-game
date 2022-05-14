using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [System.Serializable]
    public class TargetedHandler {
        [System.Serializable]
        public enum Mode {
            Living, Dead, Either
        }
        public float minRange, maxRange;
        public bool colliderMode = true;
        public Mode mode = Mode.Living;


        public TargetedHandler() {

        }

        public TargetedHandler(TargetedHandler data) {
            minRange = data.minRange;
            maxRange = data.maxRange;
            colliderMode = data.colliderMode;
            mode = data.mode;
        }

        public bool WouldBeValid(BehaviorInfo info, Entity target) {
            return WouldBeEligible(info, target) && WouldBeInRange(info, target);
        }

        public bool WouldBeEligible(BehaviorInfo info, Entity target) {
            Entity prevTarget = info.main.behaviorTarget;
            info.main.behaviorTarget = target;
            bool result = EligibleTarget(info);
            info.main.behaviorTarget = prevTarget;
            return result;
        }

        public bool WouldBeInRange(BehaviorInfo info, Entity target) {
            Entity prevTarget = info.main.behaviorTarget;
            info.main.behaviorTarget = target;
            bool result = InRange(info);
            info.main.behaviorTarget = prevTarget;
            return result;
        }

        public bool ValidTarget(BehaviorInfo info) {
            return EligibleTarget(info) && InRange(info);
        }

        public bool EligibleTarget(BehaviorInfo info) {
            switch(mode) {
                case Mode.Living:
                    return !info.main.TargetDeadOrGone();
                case Mode.Dead:
                    return info.main.TargetDeadButExists();
                case Mode.Either:
                    return !info.main.TargetGone();
                default:
                    return false;
            }
        }
        
        public bool InRange(BehaviorInfo info) {
            float sqr;
            if(colliderMode) {
                Collider2D thisCol = info.main.GetComponent<Collider2D>();
                Collider2D targetCol = info.main.behaviorTarget.GetComponent<Collider2D>();
                float dist = thisCol.Distance(targetCol).distance;
                sqr =  dist*dist;
            } else {
                sqr = Vector2.SqrMagnitude(info.main.behaviorTarget.transform.position - info.main.transform.position);
            }
            return sqr <= maxRange*maxRange && sqr >= minRange*minRange;
        }
    }
}