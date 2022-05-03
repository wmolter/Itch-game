using UnityEngine;
using System.Collections;

namespace Itch {
    public class JumpMovement : Movement {
        [Range(0, 1)]
        public float rangeVariation;
        [Range(1, 89)]
        public float launchAngle;
        [Range(0, 5)]
        public float windupTime = 0.5f;
        public float maxJumpError = 1;
        public Collider2D collision;

        //right now, used as a consumable flag reset every time it lands
        [HideInInspector]
        public bool landedFlag;

        bool windingUp;
        bool inAir;
        float jumpStart;
        float g;
        float t;
        float xv;
        float yv;
        Vector2 jumpDir;

        Vector2 target;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if(motionDir != Vector2.zero && !inAir && !windingUp) {
                StartCoroutine(WindUp());
            }
        }

        IEnumerator WindUp() {
            windingUp = true;
            if(windupTime  > 0)
                yield return new WaitForSeconds(windupTime);
            landedFlag = false; //maybe should be before the wait, but i'm worried about fixed update vs. regular update situation
            StartJump();
            windingUp = false;
        }

        void StartJump() {
            float dist = (1 - (rangeVariation*Random.value))*speed*speedFactor;
            float theta = launchAngle*Mathf.PI/180;
            jumpDir = motionDir;
            g = Planes.CurrentPlane.gravity;
            /*range = t*xv;
            xv = range/t;
            yv = lv*Mathf.Sin(theta);
            float t = yv/g*2;
            t = lv*Mathf.Sin(theta)/g*2;
            t = xv*Mathf.Tan(theta)/g*2;*/
            t = Mathf.Sqrt(dist*Mathf.Tan(theta)/g*2);
            xv = dist/t;
            float lv = xv/Mathf.Cos(theta);
            yv = lv*Mathf.Sin(theta);
            inAir = true;
            jumpStart = Time.time;

            target = jumpDir*dist + (Vector2)transform.position;

            Debug.Log("Jumped with target: " + target + " t = " + t + " dist = " + dist + " xv = " + xv + " yv = " + yv);
        }

        protected override void FixedUpdate() {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            Vector2 currVel = body.velocity;
            float tSoFar = Time.time - jumpStart;
            Vector2 desiredVel = Vector2.zero;
            if(inAir && tSoFar >= t) {
                inAir = false;
                landedFlag = true;
                float error = (target - (Vector2)transform.position).magnitude;
                Debug.Log("Landed at " + transform.position + " with target: " + target + " distance error " + error);
                if(error > maxJumpError)
                    transform.position = target;
                if(collision!=null)
                    collision.isTrigger = false;
            }
            if(inAir) {
                if(collision!=null)
                    collision.isTrigger = true;

                float vx = xv*jumpDir.x;
                float vy = xv*jumpDir.y + yv-g*tSoFar;
                desiredVel = new Vector2(vx, vy);
            }
            body.velocity = desiredVel;
            UpdateRotation();
        }
    }
}