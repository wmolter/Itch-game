using UnityEngine;
using System.Collections;

namespace Itch {
    public class Movement : MonoBehaviour {
        
        public float speed = 5;
        public float acceleration = 5;
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
            Vector2 desiredVel = motionDir*speed;
            float mass = GetComponent<Rigidbody2D>().mass;
            GetComponent<Rigidbody2D>().AddForce((desiredVel-currVel)*acceleration*mass, ForceMode2D.Force);
            if(rotate) {
                transform.rotation = Quaternion.FromToRotation(defaultRotation, GetComponent<Rigidbody2D>().velocity);
            }
        }
    }
}