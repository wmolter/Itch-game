using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Movement : MonoBehaviour {
        
        [System.Serializable]
        public enum SpriteMode {
            None, Flip, Rotate
        }
        public float speed = 5;
        [Range(0, 1)]
        public float speedFactor = 1;
        public float acceleration = 5;
        public float turnAcceleration = 5;
        public bool flyMode;
        public Vector2 motionDir;
        public Vector2 defaultRotation = Vector2.right;
        public SpriteMode spriteMode;

        public float StuckDuration { get { return Time.time - hitStartTime; } }
        // Use this for initialization
        void Start() {

        }
        // Update is called once per frame
        void Update() {
        }

        /*
        bool prevHit = false;
        protected void HandleStuck() {
            if(gameObject.tag == "Debug") {
                Debug.Log("Handling stuck.");
            }
            if(collidingWith != null && !prevHit) {
                hitStartTime = Time.time;
            }
            if(collidingWith == null) {
                hitStartTime = Mathf.Infinity;
            }
            prevHit = collidingWith != null;
            collidingWith = null;

        }*/

        protected virtual void FixedUpdate() {
            //HandleStuck();
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            Vector2 currVel = body.velocity;
            float currSpeed = currVel.magnitude;
            Vector2 currDir = currVel.normalized;
            Vector2 perp = new Vector2(currDir.y, -currDir.x);
            //find closest perpendicular
            if(Vector2.Dot(perp, motionDir) < 0)
                perp = -perp;
            float mag = (currDir-motionDir).sqrMagnitude*4;
            float mass = body.mass;
            float targetSpeed = speed*speedFactor;
            float sign = Mathf.Sign(Vector2.Dot(currVel, motionDir));
            if(flyMode) {
                sign = 1;
            }
            //don't turn if you're supposed to be stopping
            if(motionDir != Vector2.zero)
                body.AddForce(perp*currSpeed*Mathf.Min(turnAcceleration, turnAcceleration*mag)*mass, ForceMode2D.Force);
            else {
                targetSpeed = 0;
                sign = 1;
            }

            currDir = body.velocity.normalized;
            if(currDir == Vector2.zero)
                currDir = motionDir;

            body.AddForce(Mathf.Min(acceleration,acceleration*(targetSpeed-currSpeed))*sign*currDir*mass, ForceMode2D.Force);
            UpdateRotation();
        }

        public virtual void UpdateRotation() {
            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
            switch(spriteMode) {
                case SpriteMode.Rotate:
                    transform.rotation = Quaternion.FromToRotation(defaultRotation, velocity);
                    break;
                case SpriteMode.Flip:
                    GetComponent<SpriteRenderer>().flipX = Vector2.Dot(velocity, defaultRotation) < 0;
                    break;
                case SpriteMode.None:
                    break;
            }
        }

        [SerializeField]
        private float hitStartTime;
        private Collider2D collidingWith;
        
        private void OnCollisionEnter2D(Collision2D collision) {
            if(collidingWith == null) {
                collidingWith = collision.collider;
                hitStartTime = Time.time;
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if(collidingWith == collision.collider) {
                collidingWith = null;
                hitStartTime = float.PositiveInfinity;
            }
        }
    }
}