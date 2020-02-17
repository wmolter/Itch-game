using UnityEngine;
using System.Collections;

namespace Itch {
    [RequireComponent(typeof(Interactable))]
    public class Enterable : MonoBehaviour {

        public int destination;
        public Enterable exitPrefab;
        // Use this for initialization
        void OnEnable() {
            GetComponent<Interactable>().Interaction = Enter;
        }

        private void Start() {
            if(enabled) {
                GetComponent<Interactable>().Interaction = Enter;
            }
        }

        // Update is called once per frame
        void Update() {

        }

        public void Enter(PlayerManager player) {
            Planes.Activate(destination);
            if(exitPrefab != null) {
                Enterable newObj = Instantiate<Enterable>(exitPrefab, Planes.CurrentPlane.map.transform);
                newObj.transform.position = transform.position;
            }
            Debug.Log("entered.");
        }
    }
}