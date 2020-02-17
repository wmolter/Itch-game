using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace OneKnight.UI {
    public class ReusableElementManager : MonoBehaviour{

        public ReusableElement prefab;
        public ReusableElement[] startingSet;
        Queue<ReusableElement> available;
        List<ReusableElement> all;
        bool initialized = false;
        // Use this for initialization
        void Awake() {
            if(!initialized)
                Init();
        }

        void Init() {
            for(int i = 0; i < startingSet.Length; i++) {
                startingSet[i].Init(this);
                AddToAll(startingSet[i]);
                if(!startingSet[i].gameObject.activeSelf) {
                    AddAvailable(startingSet[i]);
                }
            }
            initialized = true;
        }

        //use when order is important
        public GameObject ObjectAt(int index) {
            if(!initialized)
                Init();
            if(all != null && index < all.Count) {
                return all[index].gameObject;
            } else
                return CreateNew();
        }

        //use for non-critical order, or when you're repositioning elements anyway
        public GameObject Next {
            get {
                if(!initialized)
                    Init();
                if(available != null && available.Count > 0) {
                    ReusableElement result = available.Dequeue();
                    return result.gameObject;
                } else {
                    return CreateNew();
                }
            }
        }

        private GameObject CreateNew() {
            ReusableElement el = Instantiate<ReusableElement>(prefab, transform);
            el.Init(this);
            AddToAll(el);
            return el.gameObject;
        }

        private void AddToAll(ReusableElement el) {
            if(all == null)
                all = new List<ReusableElement>();
            all.Add(el);
        }

        private void AddAvailable(ReusableElement el) {
            if(available == null) {
                available = new Queue<ReusableElement>();
            }
            available.Enqueue(el);
        }

        public void DisableFrom(int index) {
            if(!initialized)
                Init();
            if(all == null)
                return;
            for(int i = index; i < all.Count; i++) {
                all[i].gameObject.SetActive(false);
            }
            
        }

        public void OnDisabled(ReusableElement el) {
            AddAvailable(el);
        }

        public IEnumerator<GameObject> GetEnumerator() {
            if(!initialized)
                Init();
            for(int i = 0; all != null && i < all.Count; i++) {
                if(all[i].gameObject.activeSelf)
                    yield return all[i].gameObject;
            }
        }
    }
}