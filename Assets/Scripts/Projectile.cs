using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
    public class Projectile : MonoBehaviour {

        public bool consumeOnDamage = true;
        public bool selfDamage = false;
        public bool tribeDamage = false;
        public bool collidesWithObjects = true;
        public float range = 1;
        public float damage = 10;
        public ForceMode2D mode;
        public float speedFactor = 1;
        Vector2 origin;
        Vector2 direction;
        GameObject source;
        // Use this for initialization
        void Start() {
            gameObject.layer = collidesWithObjects ? 10 : 11;
        }

        public void OnSpawn(Vector2 direction, Vector2 origin, GameObject source) {
            this.origin = origin;
            this.direction = direction;
            transform.position = origin;
            transform.rotation = transform.rotation*Quaternion.FromToRotation(Vector2.right, direction);
            this.source = source;
            if(mode == ForceMode2D.Impulse)
                GetComponent<Rigidbody2D>().AddForce(speedFactor*direction, ForceMode2D.Impulse);
        }

        // Update is called once per frame
        void Update() {
            if(Vector2.SqrMagnitude(((Vector2)transform.position) - origin) > range*range) {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate() {
            if(mode == ForceMode2D.Force)
                GetComponent<Rigidbody2D>().AddForce(speedFactor*direction, ForceMode2D.Force);
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            if((tribeDamage || collider.gameObject.layer != source.layer) && (selfDamage || collider.gameObject != source)) {
                Health toDamage = collider.GetComponent<Health>();
                if(toDamage == null && collidesWithObjects)
                    Destroy(gameObject);
                else {
                    toDamage.Damage(damage);
                    if(consumeOnDamage) {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}