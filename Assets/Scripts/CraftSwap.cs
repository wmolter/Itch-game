using UnityEngine;
using System.Collections.Generic;
using Itch.Behavior;
using UnityEngine.Events;

namespace Itch {
    [RequireComponent(typeof(LevelRequiredAction))]
    public class CraftSwap : MonoBehaviour {
        public enum Mode {
            Sequential, Random
        }
        public Mode mode;
        public List<LevelRequiredAction.Data> possible;
        public bool randomStart;
        public bool alwaysDisable = true;
        int index = -1;
        bool firstTime = true;
        bool shouldSwap = false;


        // Use this for initialization
        void Awake() {
            index = possible.IndexOf(GetComponent<LevelRequiredAction>().data);
            firstTime = true;
        }

        private void OnEnable() {
            shouldSwap = true;
        }

        public void Swap() {
            Mode prev = mode;
            if(firstTime && randomStart)
                mode = Mode.Random;
            switch(mode) {
                case Mode.Sequential:
                    index = (index + 1) % possible.Count;
                    break;
                case Mode.Random:
                    index = (index + Random.Range(1, possible.Count+(index<0? 2: 1))) % possible.Count;
                    break;
            }
            GetComponent<LevelRequiredAction>().InitData(possible[index]);
            firstTime = false;
            mode = prev;
        }

        // Update is called once per frame
        void Update() {
            if(shouldSwap) {
                Swap();
                shouldSwap = false;
            }
            enabled = !alwaysDisable;
        }

        private void FixedUpdate() {
        }
    }
}