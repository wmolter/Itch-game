using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public class Planes : MonoBehaviour {

        static Planes instance;
        public static MapGenerator CurrentPlane { get { return instance.planes[instance.activePlane]; } }
        public static int CurrentPlaneIndex { get { return instance.activePlane; } }

        public static void Activate(int destinationLevel) {
            instance.ActivateInternal(destinationLevel);
        }

        public List<MapGenerator> planes;
        int activePlane;
        // Use this for initialization
        void Awake() {
            instance = this;
            for(int i = 0; i < planes.Count; i++) {
                if(planes[i].gameObject.activeSelf) {
                    activePlane = i;
                }
            }
        }

        private void OnDestroy() {
            instance = null;
        }

        // Update is called once per frame
        void Update() {

        }

        void ActivateInternal(int destinationLevel) {
            for(int i = 0; i < planes.Count; i++) {
                if(i == destinationLevel)
                    continue;
                planes[i].gameObject.SetActive(false);
            }
            //needs to be before setActive
            activePlane = destinationLevel;
            planes[destinationLevel].gameObject.SetActive(true);
        }
    }

}