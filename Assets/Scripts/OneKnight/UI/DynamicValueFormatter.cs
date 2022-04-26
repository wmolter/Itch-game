using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.UI {
    public delegate object DynamicValueGetter();
    public delegate string TextGetter();
    public class DynamicValueFormatter : MonoBehaviour {

        public string formatKey;
        public string format { get; set; }
        public List<DynamicValueGetter> getters;

        public void Add(DynamicValueGetter getter) {
            if(getters == null)
                getters = new List<DynamicValueGetter>();
            getters.Add(getter);
        }

        public void Clear() {
            if(getters != null)
                getters.Clear();
        }
        
        public string Text { get {
                if(format == null)
                    format = Strings.Get(formatKey);
                object[] values = new object[getters.Count];
                for(int i = 0; i < values.Length; i++) {
                    values[i] = getters[i]();
                }
                return string.Format(format, values);
            } }

        public TextGetter TextGetter {
            get {
                return GetText;
            }
        }
        

        public string GetText() {
            return Text;
        }
    }
}