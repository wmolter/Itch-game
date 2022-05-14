using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    public class Stunnable : MonoBehaviour {
        [Range(0, 2)]
        public float durationFactor = 1;

        public List<MonoBehaviour> disable;

        private List<MonoBehaviour> reenable;
        private List<MonoBehaviour> Reenable { get {
                if(reenable == null)
                    reenable = new List<MonoBehaviour>(disable.Count);
                return reenable;
            } }

        private Coroutine wait;
        
        public void Stun(float duration) {
            foreach(MonoBehaviour toDisable in disable) {
                if(toDisable.enabled) {
                    toDisable.enabled = false;
                    Reenable.Add(toDisable);
                }
            }
            wait = StartCoroutine(WaitEndStun(duration*durationFactor));
        }

        IEnumerator<object> WaitEndStun(float trueDuration) {
            yield return new WaitForSeconds(trueDuration);
            EndStun();
        }

        private void EndStun() {
            if(wait != null)
                StopCoroutine(wait);
            foreach(MonoBehaviour toEnable in Reenable)
                toEnable.enabled = true;
            reenable.Clear();
        }
        
        private void OnDisable() {
            EndStun();
        }
    }
}