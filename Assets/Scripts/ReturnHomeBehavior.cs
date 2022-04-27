﻿using UnityEngine;
using System.Collections;

namespace Itch {
    public class ReturnHomeBehavior : BehaviorTreeNode {

        public float allowedRange;
        public float returnCompleteRange;

        public override bool Decide() {
            return allowedRange*allowedRange < Vector2.SqrMagnitude(transform.position - mainControl.Home);
        }

        public override void DoBehavior() {
            moveControl.motionDir = (mainControl.Home - transform.position).normalized;
        }

        public override bool CheckEnd() {
            return Vector2.SqrMagnitude(transform.position - mainControl.Home) < returnCompleteRange*returnCompleteRange;
        }
    }
}