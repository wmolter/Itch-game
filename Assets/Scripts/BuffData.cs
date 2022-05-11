using UnityEngine;
using System.Collections.Generic;

namespace Itch {
    [CreateAssetMenu(menuName ="Generation/Buff Data")]
    public class BuffData : ScriptableObject {

        public List<Info> all;
        [System.Serializable]
        public struct Info {
            public string name;
            public float strength;
            public float duration;
        }

        public void Apply(PlayerManager p) {
            foreach(Info buff in all) {
                p.GiveBuff(buff.name, buff.strength, buff.duration);
            }
        }

    }
}