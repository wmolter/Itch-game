using UnityEngine;
using System.Collections.Generic;
namespace OneKnight {
    public static class Sprites {
        public const string folder = "Sprites/";
        public const string Default = "default";
        

        public static string Tier(string main, int tier) {
            string result = main + "0" + tier;
            return result;
        }

        static Dictionary<string, Sprite> all;

        public static void Load() {
            all = new Dictionary<string, Sprite>();
            Sprite[] main = Resources.LoadAll<Sprite>(folder + "main_spritesheet");
            foreach(Sprite s in main) {
                all[s.name] = s;
            }
            all["default"] = Resources.Load<Sprite>(folder + Default);
        }

        public static Sprite Get(string name) {
            if(all.ContainsKey(name))
                return all[name];
            Debug.LogWarning("Sprite " + name + " missing; replaced with default.");
            return all["default"];
        }
    }
}
