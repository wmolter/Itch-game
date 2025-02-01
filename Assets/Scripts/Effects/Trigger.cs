using UnityEngine;
using System.Collections.Generic;
using OneKnight.PropertyManagement;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Trigger")]
    public class Trigger : Effect {
        
        public new class State : Effect.State {
            public State(Effect data) : base(data) {}

            protected override void Apply(PlayerManager p, float strength) {
                foreach(Effect e in ((Trigger)activeNow[0]).toTrigger){
                    p.StartEffect(e);
                }
                foreach(PropertyAdjustment adj in ((Trigger)activeNow[0]).adjustments) {
                    p.properties.AddAdjustment(adj);
                    p.MakeLevelChanges(adj.property);
                }

            }

            protected override void Cancel(PlayerManager p) {
                foreach(PropertyAdjustment adj in ((Trigger)activeNow[0]).adjustments) {
                    p.properties.RemoveAdjustment(adj);
                    p.MakeLevelChanges(adj.property);
                }
            }
        }

        public List<Effect> toTrigger;
        public List<PropertyAdjustment> adjustments;

        public override Effect.State Create() {
            return new State(this);
        }

        public override Effect Copy() {
            return new Trigger() {}.CopyFrom(this);
        }
        
    }
}