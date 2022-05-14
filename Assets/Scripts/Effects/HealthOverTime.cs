using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Health Over Time")]
    public class HealthOverTime : Effect {
        public new class State : Effect.State{
            
            public State(string name) : this(name, true) { }

            public State(string name, bool positive) : base(name, positive) {
            }

            public override void Apply(PlayerManager p) {
                HealthOverTime toUse = (HealthOverTime)activeNow[Utils.ArgMax(activeNow, GetStrength)];
                float strengthToUse = toUse.strength;
                if(!positive)
                    strengthToUse = -strengthToUse;
                p.GetComponent<Health>().RemoveOverTime(buffName);
                p.GetComponent<Health>().AddOverTime(strengthToUse, toUse.interval, buffName, p.GetComponent<Entity>());
            }

            public override void Cancel(PlayerManager p) {
                p.GetComponent<Health>().RemoveOverTime(buffName);
            }
        }

        public float interval = 1;
        public bool nameInNotif = false;
        
        public override string NotifyText {
            get {
                string result = (positive ? "+" + strength + " regeneration for " + duration + " seconds" :
                        strength + " poison for " +  duration + " seconds");
                result = nameInNotif ? effectName + ". " + result : result;
                return result;
            }
        }

        public override Effect.State Create() {
            return new State(effectName, positive);
        }
        public override Effect Copy() {
            return new ChangeLevel().CopyFrom(this);
        }
    }
}
