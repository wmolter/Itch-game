using UnityEngine;
using System.Collections;
using TMPro;

namespace OneKnight.UI {
    [RequireComponent(typeof(TextLoader))]
    public class LoadingHint : MonoBehaviour {

        public int maxCount = 3;
        public float delay = 1f;
        public string toAdd = ".";

        private string baseString;
        private int count = 0;
        private float nextTime;
        // Use this for initialization
        void Start() {
            baseString = GetComponent<TextLoader>().text;
        }

        // Update is called once per frame
        void Update() {
            if(Time.time >= nextTime) {
                string cur = baseString + "";
                for(int i = 0; i <= count; i++) {
                    cur += toAdd;
                }
                GetComponent<TextMeshProUGUI>().text = cur;
                nextTime = Time.time + delay;
                count = ((count + 1) % maxCount);
                
            }
        }
    }
}