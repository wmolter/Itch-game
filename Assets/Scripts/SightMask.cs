using UnityEngine;
using System.Collections;

namespace Itch {
    public class SightMask : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            float los = Planes.CurrentPlane.SightModifier * PlayerManager.instance.LOS;
            transform.localScale = Vector3.one*los*2;
        }
    }
}