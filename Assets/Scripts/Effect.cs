using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    public abstract class Effect : ScriptableObject {
        public abstract class State {
            public string buffName { get; private set; }
            public List<Effect> activeNow;
            public List<float> endTimes;
            public bool positive;

            public bool Empty {
                get {
                    return activeNow.Count == 0;
                }
            }

            public State(string name) : this(name, true) { }

            public State(string name, bool positive) {
                buffName = name;
                activeNow = new List<Effect>();
                endTimes = new List<float>();
                this.positive = positive;
            }

            public void Add(PlayerManager p, Effect active) {
                activeNow.Add(active);
                endTimes.Add(Time.time + active.duration);
                Apply(p);
            }

            public void UpdateBuffs(PlayerManager p) {
                int index = 0;
                while(index < activeNow.Count) {
                    if(endTimes[index] <= Time.time) {
                        activeNow.RemoveAt(index);
                        endTimes.RemoveAt(index);
                    } else {
                        index++;
                    }
                }
                if(activeNow.Count > 0) {
                    Apply(p);
                } else {
                    Cancel(p);
                }
            }

            public abstract void Apply(PlayerManager p);

            public abstract void Cancel(PlayerManager p);
        }

        public string effectName;
        public float duration;
        public bool positive = true;

        public abstract State Create();

        public virtual string NotifyText { get { return effectName + " for " + duration + " seconds"; } }

        public virtual void Notify(Vector2 pos) {
            if(positive) {
                Notifications.CreatePositive(pos, NotifyText);
            } else {
                Notifications.CreateNegative(pos, NotifyText);
            }
        }

        public virtual bool Match(State manager) {
            return manager.buffName == effectName && !(positive ^ manager.positive);
        }
    }
}
