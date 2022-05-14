using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Itch {
    public class PlayerController : MonoBehaviour {

        public float acceleration;
        public float modifier = 1;
        private float trueSpeed;
        

        Vector2 motionDir;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            trueSpeed = GetComponent<PlayerManager>().Speed;
        }

        public void OnMove(InputValue value) {
            motionDir = value.Get<Vector2>().normalized;
        }

        private void FixedUpdate() {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            Vector2 currVel = body.velocity;
            Vector2 desiredVel = motionDir*trueSpeed*modifier;
            float mass = GetComponent<Rigidbody2D>().mass;
            GetComponent<Rigidbody2D>().AddForce((desiredVel-currVel)*acceleration*mass, ForceMode2D.Force);
        }
    }
}