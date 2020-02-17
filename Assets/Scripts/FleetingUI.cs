using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Itch {
    [RequireComponent(typeof(CanvasGroup))]
    public class FleetingUI : MonoBehaviour {

        public float rate;
        public Vector2 dir;
        public float fullAlphaTime;
        public float fadeTime;
        float startTime;
        // Use this for initialization
        void Start() {
            startTime = Time.time;
        }

        // Update is called once per frame
        void Update() {
            transform.position = (Vector2)transform.position + (rate*dir*Time.deltaTime);
            GetComponent<CanvasGroup>().alpha = (1 - (Time.time - (startTime + fullAlphaTime))/fadeTime);
            if(Time.time >= startTime + fullAlphaTime + fadeTime) {
                Destroy(gameObject);
            }
        }
    }
}