using UnityEngine;
using System.Collections.Generic;
using OneKnight.Loading;

namespace OneKnight {
    public static class Preferences {

        public const string filename = "preferences.txt";
        private static Dictionary<string, object> prefs;
        private static List<string> changed;

        public static void Load() {
            prefs = new Dictionary<string, object>();
            SavingUtils.ReadDict(SavingUtils.ReadOKTable(filename).GetEnumerator(), prefs);
            Language = (string)prefs["language"];
        }

        public static void Save() {
            SavingUtils.WriteDictEntries(filename, prefs, changed);
        }

        public static string LanguageFileModifier { get { return Language; } }

        public static string Language {
            get; set;
        }

        public static bool Has(string name) {
            return prefs.ContainsKey(name);
        }

        public static bool GetToggle(string name) {
            return (bool)prefs[name];
        }

        public static object Get(string name) {
            return prefs[name];
        }

        public static T Get<T>(string name) {
            return (T)prefs[name];
        }

        public static void UpdateAdd(string name, object value) {
            prefs[name] = value;
            if(changed == null) {
                changed = new List<string>();
            }
            changed.Add(name);
        }
    }
}