using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Change Level")]
    public class ChangeLevel : Effect {
        public new class State : Effect.State{
            public string sourceId = "buff";
            
            public State(Effect data) : base(data) {
                if(positive) {
                    sourceId = "buff";
                } else {
                    sourceId = "debuff";
                }
            }

            protected override void Apply(PlayerManager p, float strength) {
                if(p.properties.HasAdjustment(buffName))
                    p.properties.RemoveAdjustment(buffName, sourceId);
                if(!positive)
                    strength = -strength;
                p.properties.AddBonus(buffName, strength, sourceId);
                p.MakeLevelChanges(buffName);
            }

            protected override void Cancel(PlayerManager p) {
                if(p.properties.HasAdjustment(buffName))
                    p.properties.RemoveAdjustment(buffName, sourceId);
                p.MakeLevelChanges(buffName);
            }
        }
        
        public override string NotifyText { get { return (positive? "+" : "-") + strength + " " + effectName + " for " + duration + " seconds"; } }
        public override Effect.State Create() {
            return new State(this);
        }
        public override Effect Copy() {
            return new ChangeLevel().CopyFrom(this);
        }
    }
}
