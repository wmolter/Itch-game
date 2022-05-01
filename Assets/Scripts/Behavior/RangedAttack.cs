using UnityEngine;
using System.Collections;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Ranged Attack")]
    public class RangedAttack : Attack {
        protected new class Act : Attack.Act {
            private RangedAttack Data { get { return (RangedAttack)data; } }
            public Act(RangedAttack data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            protected override void Attack() {
                //Collider2D hit = Physics2D.OverlapCircle(info.move.transform.position, Data.fireRange, LayerMask.GetMask(info.main.enemyLayers.ToArray()));
                Collider2D target = info.main.behaviorTarget.GetComponent<Collider2D>(); //maybe behavior target should be collider? idk
                if(target == null) {
                    preparing = false;
                    return;
                }
                Vector2 currentAim = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
                foreach(RangedAttackData attack in Data.attacks) {
                    Projectile prefab = Resources.Load<Projectile>("Prefabs/Projectiles/" + attack.projectileName);
                    Vector2 predictAim = Vector2.right;
                    if(attack.aim == AimMode.Predict) {
                        predictAim = Utils.PredictAim(target.transform.position, target.attachedRigidbody.velocity, transform.position, prefab.speedFactor);
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
                        p.OnSpawn(dir, origin, info.main.GetComponent<Entity>());
                    }
                }
            }
        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }

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
        public bool returnOnCooldown = true;

        
    }
}