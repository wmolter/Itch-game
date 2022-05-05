using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch.Behavior {
    [CreateAssetMenu(menuName = "Behavior/Tile Type Trigger")]
    public class TileTypeTrigger : Selector {
        protected new class Act : Selector.Act {
            private TileTypeTrigger Data { get { return (TileTypeTrigger)data; } }

            public Act(TileTypeTrigger data, ActiveNode parent, int index) : base(data, parent, index) {

            }

            public override bool Decide(BehaviorInfo info) {
                Debug.Log("Tile trigger decide called.");
                return (Data.tile == info.main.GetComponent<Entity>().CurrentTileType) && base.Decide(info);
            }

        }

        protected override ActiveNode CreateActive(ActiveNode parent, int index) {
            return new Act(this, parent, index);
        }
        
        public int tile;
    }
}