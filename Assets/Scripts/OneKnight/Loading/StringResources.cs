using UnityEngine;
using System.Collections.Generic;


namespace OneKnight.Loading {
    public static class StringResources {
        public const string filebase = "description_strings_";

        public static void Load() {
            substitutionKeys = new List<string>();
            substitutions = new List<string>();
            IEnumerator<SavingUtils.TableBit> enumerator = SavingUtils.ReadOKTable(Application.dataPath + "/substitutions.oktable").GetEnumerator();
            while(enumerator.MoveNext()) {
                substitutionKeys.Add(enumerator.Current.value);
                enumerator.MoveNext();
                substitutions.Add(enumerator.Current.value);
            }

            allStrings = new Dictionary<string, string>();
            enumerator = SavingUtils.ReadOKTable(Application.dataPath + "/" + filebase + Preferences.LanguageFileModifier + ".oktable").GetEnumerator();
            while(enumerator.MoveNext()) {
                string key = enumerator.Current.value;
                if(key.StartsWith("#")) {
                    enumerator.MoveNext();
                    substitutions.Add(Substitute(enumerator.Current.value));
                    substitutionKeys.Add(key);
                } else {
                    enumerator.MoveNext();
                    allStrings[key] = Substitute(enumerator.Current.value);
                }
            }
        }

        private static Dictionary<string, string> allStrings;
        private static List<string> substitutionKeys;
        private static List<string> substitutions;

        public static string Get(string key) {
            if(allStrings.ContainsKey(key))
                return allStrings[key];
            else {
                Debug.LogWarning("Could not find localized string: " + key + " for language: " + Preferences.Language);
                return key;
            }
        }

        public static string Substitute(string raw) {
            for(int i = 0; i < substitutionKeys.Count; i++) {
                raw = raw.Replace(substitutionKeys[i], substitutions[i]);
            }
            return raw;
        }

        public static string Format(string key, params object[] args) {
            return string.Format(Get(key), args);
        }

    }
}
