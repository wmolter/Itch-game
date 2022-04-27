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
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

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
            body.AddForce(perp*currSpeed*Mathf.Min(turnAcceleration, turnAcceleration*mag)*mass, ForceMode2D.Force);

            //use just updated velocity 
            float signedCurrSpeed = currSpeed*Mathf.Sign(Vector2.Dot(currVel, motionDir));
            currDir = body.velocity.normalized;
            if(currDir == Vector2.zero)
                currDir = motionDir;
            Vector2 desiredVel = motionDir*speed;

            body.AddForce(Mathf.Min(acceleration,acceleration*(speed-signedCurrSpeed))*currDir*mass, ForceMode2D.Force);

            if(rotate) {
                transform.rotation = Quaternion.FromToRotation(defaultRotation, body.velocity);
            }
        }
    }
}