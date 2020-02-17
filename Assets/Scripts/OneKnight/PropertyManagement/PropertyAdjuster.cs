using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.PropertyManagement {
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


        public void AddModifier(float mod, int source) {
            AddAdjustment(new PropertyAdjustment(PropertyAdjustment.Type.Modifier, mod, source));
        }

        public void AddBonus(float bonus, int source) {
            AddAdjustment(new PropertyAdjustment(PropertyAdjustment.Type.Bonus, bonus, source));
        }

        public void AddMax(float max, int source) {
            AddAdjustment(new PropertyAdjustment(PropertyAdjustment.Type.Max, max, source));
        }

        public void AddMin(float min, int source) {
            AddAdjustment(new PropertyAdjustment(PropertyAdjustment.Type.Min, min, source));
        }

        public void AddAdjustment(PropertyAdjustment adjustment) {
            if(adjustments[(int)adjustment.type] == null)
                adjustments[(int)adjustment.type] = new List<PropertyAdjustment>();
            if(!adjustments[(int)adjustment.type].Contains(adjustment))
                adjustments[(int)adjustment.type].Add(adjustment);
        }

        public void RemoveAdjustment(int sourceID) {
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

        public void RemoveAdjustment(string sourceName) {
            for(int i = 0; i < adjustments.Length; i++) {
                if(adjustments[i] == null)
                    continue;
                for(int j = 0; j < adjustments[i].Count; j++) {
                    if(adjustments[i][j].sourceName == sourceName) {
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