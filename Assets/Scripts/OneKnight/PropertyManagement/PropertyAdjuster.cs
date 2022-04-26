using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.PropertyManagement {
    [System.Serializable]
    public class PropertyAdjuster {

        List<PropertyAdjustment>[] adjustments;
        string property;

        public bool HasAdjustment {
            get {
                for(int i = 0; i < adjustments.Length; i++) {
                    if(adjustments[i] != null && adjustments[i].Count > 0)
                        return true;
                }
                return false;
            }
        }

        public PropertyAdjuster(string property) {
            this.property = property;
            adjustments = new List<PropertyAdjustment>[4];
        }
        
        public float Adjust(float value) {
            float result = value;
            foreach(List<PropertyAdjustment> op in adjustments) {
                if(op != null) {
                    foreach(PropertyAdjustment adjustment in op)
                        result = adjustment.Apply(result, value);
                }
            }
            return result;
        }
        
        public void AddModifier(float mod, string source) {
            AddAdjustment(new PropertyAdjustment(property, PropertyAdjustment.Type.Modifier, mod, source));
        }

        public void AddBonus(float bonus, string source) {
            AddAdjustment(new PropertyAdjustment(property, PropertyAdjustment.Type.Bonus, bonus, source));
        }

        public void AddMax(float max, string source) {
            AddAdjustment(new PropertyAdjustment(property, PropertyAdjustment.Type.Max, max, source));
        }

        public void AddMin(float min, string source) {
            AddAdjustment(new PropertyAdjustment(property, PropertyAdjustment.Type.Min, min, source));
        }

        public void AddAdjustment(PropertyAdjustment adjustment) {
            if(adjustment.property != property)
                throw new UnityException("Adjustment must have the same property as the adjuster.");
            if(adjustments[(int)adjustment.type] == null)
                adjustments[(int)adjustment.type] = new List<PropertyAdjustment>();
            if(adjustments[(int)adjustment.type].Contains(adjustment))
                adjustments[(int)adjustment.type].Remove(adjustment);
            adjustments[(int)adjustment.type].Add(adjustment);
        }

        private PropertyAdjustment FindAdjustment(string source) {
            for(int i = 0; i < adjustments.Length; i++) {
                if(adjustments[i] == null)
                    continue;
                for(int j = 0; j < adjustments[i].Count; j++) {
                    if(adjustments[i][j].sourceID == source) {
                        return adjustments[i][j];
                    }
                }
            }
            return null;
        }

        public void RemoveAdjustment(string sourceID) {
            for(int i = 0; i < adjustments.Length; i++) {
                if(adjustments[i] == null)
                    continue;
                for(int j = 0; j < adjustments[i].Count; j++) {
                    if(adjustments[i][j].sourceID == sourceID) {
                        //must remove all adjustments from the source
                        adjustments[i].RemoveAt(j);
                    }
                }
            }
        }

        public string Details(float value) {
            string result = "";
            float finalValue = value;
            bool first = true;
            foreach(List<PropertyAdjustment> op in adjustments) {
                if(op != null) {
                    foreach(PropertyAdjustment adjustment in op) {
                        if(!first)
                            result += "\n";
                        result += adjustment.Details(finalValue, value);
                        finalValue = adjustment.Apply(finalValue, value);
                        first = false;
                    }
                }
            }
            return result;
        }
    }
}