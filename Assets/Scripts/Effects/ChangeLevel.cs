using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Change Level")]
    public class ChangeLevel : Effect {
        public new class State : Effect.State{
            public string sourceId = "buff";
            
            public State(string name) : this(name, true) { }

            public State(string name, bool positive) : base(name, positive) {
                if(positive) {
                    sourceId = "buff";
                } else {
                    sourceId = "debuff";
                }
            }

            public override void Apply(PlayerManager p) {
                if(p.properties.HasAdjustment(buffName))
                    p.properties.RemoveAdjustment(buffName, sourceId);
                float strengthToUse = Utils.Max(activeNow, GetStrength);
                if(!positive)
                    strengthToUse = -strengthToUse;
                p.properties.AddBonus(buffName, strengthToUse, sourceId);
                p.MakeLevelChanges(buffName);
            }

            public override void Cancel(PlayerManager p) {
                if(p.properties.HasAdjustment(buffName))
                    p.properties.RemoveAdjustment(buffName, sourceId);
                p.MakeLevelChanges(buffName);
            }
        }
        
        public override string NotifyText { get { return (positive? "+" : "-") + strength + " " + effectName + " for " + duration + " seconds"; } }
        public override Effect.State Create() {
            return new State(effectName, positive);
        }
        public override Effect Copy() {
            return new ChangeLevel().CopyFrom(this);
        }
    }
}
