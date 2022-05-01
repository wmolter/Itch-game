﻿using UnityEngine;
using System.Collections;

namespace Itch.Behavior {
    public abstract class RelationalMovement : BehaviorNode {
        protected new abstract class Act : ActiveNode {
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
                return info.main.behaviorTarget == null || Vector2.SqrMagnitude(info.main.behaviorTarget.transform.position - info.main.transform.position) > Data.endAggroRange*Data.endAggroRange;
            }

            public override void OnSuspend(BehaviorInfo info) {
                info.move.motionDir = Vector2.zero;
            }

            public abstract Vector2 MoveDirection(BehaviorInfo info, Transform relation);
            
        }
        public float endAggroRange;
        
    }
    
}