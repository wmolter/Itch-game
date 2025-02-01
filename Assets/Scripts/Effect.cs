using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    public abstract class Effect : ScriptableObject {
        protected static float GetOneStrength(Effect e) {
            return e.strength;
        }
        public abstract class State {
            public string buffName { get; private set; }
            public List<Effect> activeNow;
            public List<float> endTimes;
            public bool positive;
            public bool stacking;
            public float threshold = 0;
            protected bool applied;

            public bool Empty {
                get {
                    return activeNow.Count == 0;
                }
            }

            public State(Effect data) : this(data.effectName, data.positive, data.stacking) {

                threshold = data.threshold;
            }

            public State(string name) : this(name, true, false) { }

            public State(string name, bool positive, bool stacking) {
                buffName = name;
                activeNow = new List<Effect>();
                endTimes = new List<float>();
                this.positive = positive;
                this.stacking = stacking;
            }

            public void Add(PlayerManager p, Effect active) {
                activeNow.Add(active);
                endTimes.Add(Time.time + active.duration);
                TryApply(p);
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
                if(activeNow.Count > 0 && TryApply(p)){
                    //empty on purpose - things happen in tryapply
                } else if(applied) {
                    Cancel(p);
                    applied = false;
                }
            }

            public bool TryApply(PlayerManager p) {
                float strength = GetStrength();
                if(strength >= threshold) {
                    Apply(p, strength);
                    applied = true;
                    return true;
                }
                return false;
            }

            public virtual float GetStrength() {
                if(stacking) {
                    return Utils.Sum(activeNow, GetOneStrength);
                }
                return Utils.Max(activeNow, GetOneStrength);
            }

            protected abstract void Apply(PlayerManager p, float strength);

            protected abstract void Cancel(PlayerManager p);
        }

        public string effectName;
        public float duration;
        public float strength;
        public float threshold = 0;
        public bool positive = true;
        public bool stacking = false;

        public abstract State Create();
        public abstract Effect Copy();

        public Effect CopyInto(Effect e) {
            e.effectName = effectName;
            e.duration = duration;
            e.strength = strength;
            e.positive = positive;
            e.stacking = stacking;
            e.threshold = threshold;
            return e;
        }

        public Effect CopyFrom(Effect e) {
            effectName = e.effectName;
            duration = e.duration;
            strength = e.strength;
            positive = e.positive;
            stacking = e.stacking;
            threshold = e.threshold;
            return this;
        }

        public virtual string NotifyText { get { return effectName + " for " + duration + " seconds"; } }

        public virtual void Notify(Vector2 pos) {
            if(positive) {
                Notifications.CreatePositive(pos, NotifyText);
            } else {
                Notifications.CreateNegative(pos, NotifyText);
            }
        }

        public virtual bool Match(State manager) {
            return manager.buffName == effectName && !(positive ^ manager.positive) && !(stacking ^ manager.stacking);
        }
    }
}
