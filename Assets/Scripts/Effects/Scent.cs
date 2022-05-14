using UnityEngine;
using System.Collections;

namespace Itch.Effects {
    [CreateAssetMenu(menuName = "Effects/Scent")]
    public class Scent : Effect {
        
        public new class State : Effect.State {
            public State(bool positive) : base("Scent", positive) {}

            public override void Apply(PlayerManager p) {
                p.gameObject.layer = LayerMask.NameToLayer(((Scent)activeNow[activeNow.Count-1]).layerName);
            }

            public override void Cancel(PlayerManager p) {
                p.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }

        public string layerName;

        public override Effect.State Create() {
            return new State(positive);
        }

        public override Effect Copy() {
            return new Scent() {layerName = layerName }.CopyFrom(this);
        }

        //only one scent at a time, therefore all are managed by one. Could also string match "Scent" name...
        public override bool Match(Effect.State manager) {
            return manager is State;
        }
    }
}