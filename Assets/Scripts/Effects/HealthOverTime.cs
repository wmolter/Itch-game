using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Health Over Time")]
    public class HealthOverTime : Effect {
        public new class State : Effect.State{
            
            public State(Effect data) : base(data) { }

            protected override void Apply(PlayerManager p, float strength) {
                HealthOverTime toUse = (HealthOverTime)activeNow[Utils.ArgMax(activeNow, GetOneStrength)];
                //slight waste here in getting the strength again, but this helps support stacking so...
                if(!positive)
                    strength = -strength;
                p.GetComponent<Health>().RemoveOverTime(buffName);
                p.GetComponent<Health>().AddOverTime(strength, toUse.interval, buffName, p.GetComponent<Entity>());
            }

            protected override void Cancel(PlayerManager p) {
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
            return new State(this);
        }
        public override Effect Copy() {
            return new ChangeLevel().CopyFrom(this);
        }
    }
}
