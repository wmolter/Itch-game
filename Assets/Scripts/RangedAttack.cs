using UnityEngine;
using System.Collections;

namespace Itch {
    public class RangedAttack : BehaviorTreeNode {
        [System.Serializable]
        public enum SpreadMode {
            Radial, Linear
        }

        [System.Serializable]
        public enum AimMode {
            None, Current, Predict
        }

        [System.Serializable]
        public struct RangedAttackData {
            public string projectileName;
            public int quantity;
            public SpreadMode mode;
            public float spreadArc;
            public float spreadDistance;
            public float attackAngle; 
            public AimMode aim;
        }

        public RangedAttackData[] attacks;
        public float attackTime;
        public float cooldown;
        public float fireRange = 5;
        public bool returnOnCooldown;

        float nextAttackTime;
        bool attacking;

        public override bool Decide() {
            return CanAttack() && EnemiesInRange();
        }

        public bool EnemiesInRange() {
            return Physics2D.OverlapCircle(transform.position, fireRange, LayerMask.GetMask(mainControl.enemyLayers.ToArray())) != null;
        }

        public bool CanAttack() {
            return Time.time >= nextAttackTime;
        }

        public override void DoBehavior() {
            moveControl.motionDir = Vector2.zero;
            if(!attacking && CanAttack()) {
                StartCoroutine(Attack());
            }
        }

        IEnumerator Attack() {
            attacking = true;
            yield return new WaitForSeconds(attackTime);
            Collider2D hit = Physics2D.OverlapCircle(transform.position, fireRange, LayerMask.GetMask(mainControl.enemyLayers.ToArray()));

            if(hit == null)
                yield break;
            Vector2 currentAim = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            foreach(RangedAttackData attack in attacks) {
                Projectile prefab = Resources.Load<Projectile>("Prefabs/Projectiles/" + attack.projectileName);
                Vector2 predictAim = Vector2.right;
                if(attack.aim == AimMode.Predict) {
                    predictAim = PredictAim(hit.transform.position, hit.attachedRigidbody.velocity, transform.position, prefab.speedFactor);
                    //fallback to aiming where they are if they would outrun the projectile.
                    if(predictAim == Vector2.zero) {
                        predictAim = currentAim;
                    }
                }

                for(int i = 0; i < attack.quantity; i++) {
                    Projectile p = Instantiate(prefab);
                    Vector2 dir, origin;
                    switch(attack.mode) {
                        case SpreadMode.Radial:
                            //spread them out around the center -- it won't quite reach the full angle, but that's okay
                            float angle = (i - (attack.quantity-1)/2f)*attack.spreadArc/(attack.quantity);
                            origin = transform.position;
                            switch(attack.aim) {
                                case AimMode.Current:
                                    angle += Vector2.SignedAngle(Vector2.right, currentAim);
                                    break;
                                case AimMode.None:
                                    angle += attack.attackAngle;
                                    break;
                                case AimMode.Predict:
                                    angle += Vector2.SignedAngle(Vector2.right, predictAim);
                                    break;
                            }
                            angle *= Mathf.PI/180;
                            dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            break;
                        case SpreadMode.Linear:
                            switch(attack.aim) {
                                case AimMode.Current:
                                    dir = currentAim;
                                    break;
                                case AimMode.None:
                                    dir = new Vector2(Mathf.Cos(Mathf.PI/180*attack.attackAngle), Mathf.Sin(Mathf.PI/180*attack.attackAngle));
                                    break;
                                case AimMode.Predict:
                                    dir = predictAim;
                                    break;
                                default:
                                    dir = Vector2.right;
                                    break;
                            }
                            float offsetDist;
                            if(attack.quantity == 1)
                                offsetDist = 0;
                            else
                                offsetDist = (i - (attack.quantity-1)/2f) * attack.spreadDistance/(attack.quantity-1);
                            Vector2 offsetDir = Vector2.Perpendicular(dir);
                            origin = offsetDist*offsetDir + (Vector2)transform.position;
                            Debug.Log("Aim: " + dir + " offsetDir: " + offsetDir + " origin: " + origin);
                            break;
                        default:
                            dir = Vector2.right;
                            origin = transform.position;
                            break;
                    }
                    p.OnSpawn(dir, origin, GetComponent<Entity>());
                }
            }

            attacking = false;
            nextAttackTime = Time.time + cooldown;
        }

        public Vector2 PredictAim(Vector2 enemyPos, Vector2 enemyVelocity, Vector2 origin, float projectileSpeed) {
            float x0 = enemyPos.x;
            float y0 = enemyPos.y;
            float x1 = origin.x;
            float y1 = origin.y;
            float vx0 = enemyVelocity.x;
            float vy0 = enemyVelocity.y;
            float v = projectileSpeed;

            float ax = vx0*vx0;
            float ay = vy0*vy0;
            float bx = 2*(x0-x1)*vx0;
            float by = 2*(y0-y1)*vy0;
            float cx = (x0-x1)*(x0-x1);
            float cy = (y0-y1)*(y0-y1);

            float a = (-v*v + ax + ay);
            float b = bx + by;
            float c = cx + cy;

            float t = (-b + Mathf.Sqrt(b*b - 4*a*c))/(2*a);
            if(t < 0)
                t = (-b - Mathf.Sqrt(b*b - 4*a*c))/(2*a);

            float vx1 = vx0 + (x0-x1)/t;
            float vy1 = vy0 + (y0-y1)/t;
            //Debug.Log("Predicted dir: " + vx1 + ", " + vy1 + " with t: " + t + " for " + enemyPos + " going " + enemyVelocity + " from " + origin + " with speed: " + projectileSpeed);
            //if you input NaNs to this, it will become 0s
            return new Vector2(vx1, vy1).normalized;

        }

        public override bool CheckEnd() {
            return !attacking && (returnOnCooldown || !EnemiesInRange());
        }
        
    }
}