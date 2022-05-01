using UnityEngine;
using System.Collections;

namespace Itch {
    public class Movement : MonoBehaviour {
        
        public float speed = 5;
        public float acceleration = 5;
        public float turnAcceleration = 5;
        public Vector2 motionDir;
        public Vector2 defaultRotation = Vector2.right;
        public bool rotate;

        public float StuckDuration { get { return Time.time - hitStartTime; } }
        // Use this for initialization
        void Start() {

        }

        bool prevHit = false;
        // Update is called once per frame
        void Update() {
            //i could check whether this is the same as before, but really what's the point
            if(collidingWith != null && !prevHit) {
                hitStartTime = Time.time;
            }
            if(collidingWith == null) {
                hitStartTime = Mathf.Infinity;
            }
            prevHit = collidingWith == null;
            collidingWith = null;
        }

        protected virtual void FixedUpdate() {
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
            float targetSpeed = speed;
            //don't turn if you're supposed to be stopping
            float sign = Mathf.Sign(Vector2.Dot(currVel, motionDir));
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

            if(rotate) {
                transform.rotation = Quaternion.FromToRotation(defaultRotation, body.velocity);
            }
        }

        private float hitStartTime;
        private Collider2D collidingWith;
        
        private void OnCollisionEnter2D(Collision2D collision) {
            collidingWith = collision.collider;
        }
    }
}