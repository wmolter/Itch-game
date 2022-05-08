using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [System.Serializable]
    public class TargetedHandler {
        public float minRange, maxRange;
        public bool colliderMode = true;
        public bool requireLiving = true;

        public TargetedHandler() {

        }

        public TargetedHandler(TargetedHandler data) {
            minRange = data.minRange;
            maxRange = data.maxRange;
            colliderMode = data.colliderMode;
            requireLiving = data.requireLiving;
        }

        public bool ValidTarget(BehaviorInfo info) {
            return EligibleTarget(info) && InRange(info);
        }

        public bool EligibleTarget(BehaviorInfo info) {
            if(requireLiving) {
                return !info.main.TargetDeadOrGone();
            }
            return !info.main.TargetGone();
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