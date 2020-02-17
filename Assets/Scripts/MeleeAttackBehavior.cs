using UnityEngine;
using System.Collections;

namespace Itch {
    public class MeleeAttackBehavior : BehaviorTreeNode {

        public float range;
        public float attackTime;
        public float damage;
        public float cooldown;
        public bool returnOnCooldown;

        float nextAttackTime;
        bool attacking;

        public override bool Decide() {
            return Physics2D.OverlapCircle(transform.position, range, LayerMask.GetMask(mainControl.enemyLayers)) != null;
        }

        public override void DoBehavior() {
            mainControl.motionDir = Vector2.zero;
            if(!attacking && Time.time >= nextAttackTime) {
                StartCoroutine(Attack());
            }
        }

        IEnumerator Attack() {
            attacking = true;
            yield return new WaitForSeconds(attackTime);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask(mainControl.enemyLayers));
            foreach(Collider2D hit in hits) {
                hit.GetComponent<Health>().Damage(damage);
            }
            attacking = false;
            nextAttackTime = Time.time + cooldown;
        }

        public override bool CheckEnd() {
            return !attacking && (returnOnCooldown || !Decide());
        }
    }

}