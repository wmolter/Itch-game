using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    public abstract class RelationalMovement : Moving {
        protected new abstract class Act : Moving.Act {
            private RelationalMovement Data { get { return (RelationalMovement)data; } }
            public Act(RelationalMovement data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                //sometimes 
                Debug.Log("Deciding relational movement: " + info.main.behaviorTarget);
                return info.main.behaviorTarget != null && !CheckEnd(info);
            }

            public override void DoBehavior(BehaviorInfo info) {
                info.move.motionDir = (MoveDirection(info, info.main.behaviorTarget.transform)).normalized;
                //Debug.Log("New motion direction: " + info.move.motionDir);
            }

            public override bool CheckEnd(BehaviorInfo info) {
                Debug.Log("info: " + info + " info.main: " + info.main + " behaviorTarget: " + info.main.behaviorTarget + " Data: " + Data);
                if(info.main.behaviorTarget == null)
                    return true;
                if(Data.colliderMode) {
                    Collider2D thisCol = info.main.GetComponent<Collider2D>();
                    Collider2D targetCol = info.main.behaviorTarget.GetComponent<Collider2D>();
                    return thisCol.Distance(targetCol).distance > Data.endAggroRange;
                }

                return Vector2.SqrMagnitude(info.main.transform.position-info.main.behaviorTarget.transform.position) > Data.endAggroRange*Data.endAggroRange;
            }

            public abstract Vector2 MoveDirection(BehaviorInfo info, Transform relation);
            
        }
        public float endAggroRange;
        public bool colliderMode;
        
    }
    
}