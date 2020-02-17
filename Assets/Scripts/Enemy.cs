using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(NonPlayerEntity))]
    public class Enemy : MonoBehaviour {

        public BehaviorTree behavior;
        public float speed = 5;
        public float acceleration = 5;
        public Vector2 motionDir;
        public string[] enemyLayers;
        [HideInInspector]
        public Vector3 home;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void FixedUpdate() {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            Vector2 currVel = body.velocity;
            Vector2 desiredVel = motionDir*speed;
            GetComponent<Rigidbody2D>().AddForce((desiredVel-currVel)*acceleration, ForceMode2D.Force);
        }
    }
}