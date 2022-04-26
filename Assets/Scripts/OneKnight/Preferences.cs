using UnityEngine;
using System.Collections.Generic;

namespace OneKnight {
    public static class Preferences {

        public const string filename = "preferences.txt";
        private static Dictionary<string, object> prefs;

        public static void LoadPreferences() {
            prefs = new Dictionary<string, object>();
            Language = "english";
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

        public static void UpdateAdd(string name, object value) {
            prefs[name] = value;
        }
    }
}