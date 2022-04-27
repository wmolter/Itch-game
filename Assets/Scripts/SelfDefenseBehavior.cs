using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Health))]
    public class SelfDefenseBehavior : BehaviorTreeNode {

        //a long ranged attack may not trigger this behavior
        public float noticeRange = 10;
        //at 1, any damage will trigger this
        [Range(0, 1)]
        public float percentageThreshold = 1;

        Entity angryAt;

        protected virtual void Awake() {
            GetComponent<Health>().OnDamaged += NoticeAttack;
        }

        protected virtual void OnEnable (){
            mainControl.behaviorTarget = angryAt;
        }

        public override void DoBehavior() {
        }

        public override bool CheckEnd() {
            return true;
        }

        public override bool Decide() {
            return angryAt != null;
        }

        public override void OnFinish() {
            mainControl.enemyLayers.Remove(LayerMask.LayerToName(angryAt.gameObject.layer));
            angryAt = null;
        }

        private void NoticeAttack(Health.EventData data) {
            if(data.byWho != null && data.byWho.gameObject != gameObject) {
                if(data.percentage <= percentageThreshold && (data.byWho.transform.position - transform.position).magnitude <= noticeRange) {
                    angryAt = data.byWho;
                    mainControl.enemyLayers.Add(LayerMask.LayerToName(angryAt.gameObject.layer));
                }
            }
        }
    }
}