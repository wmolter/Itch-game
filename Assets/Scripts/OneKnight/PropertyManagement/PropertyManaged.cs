using UnityEngine;
using System.Collections;

namespace OneKnight.PropertyManagement {
    [System.Serializable]
    public class PropertyManaged {

        [System.NonSerialized]
        public PropertyManager Properties;

        public float AdjustProperty(string property, float value) {
            if(Properties != null) {
                return Properties.AdjustProperty(property, value);
            }
            return value;
        }

        public virtual void SetProperties(PropertyManager manager) {
            Properties = manager;
        }
    }
}