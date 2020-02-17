using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.PropertyManagement {
    [System.Serializable]
    public class PropertyManager {

        private Dictionary<string, PropertyAdjuster> adjustments;

        public PropertyManager() {
            adjustments = new Dictionary<string, PropertyAdjuster>();
        }

        public void AddAdjustment(string property, PropertyAdjustment adjustment) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddAdjustment(adjustment);
        }

        public void AddModifier(string property, float modifier, int sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddModifier(modifier, sourceID);
        }

        public void AddBonus(string property, float bonus, int sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddBonus(bonus, sourceID);
        }

        public void AddMin(string property, float min, int sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddMin(min, sourceID);
        }

        public void AddMax(string property, float max, int sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddMax(max, sourceID);
        }

        public void RemoveAdjustment(string property, int sourceID) {
            adjustments[property].RemoveAdjustment(sourceID);
        }

        public void RemoveAdjustment(string property, string sourceName) {
            adjustments[property].RemoveAdjustment(sourceName);
        }

        public bool HasAdjustment(string property) {
            if(adjustments.ContainsKey(property))
                return adjustments[property].HasAdjustment;
            return false;
        }

        public string AdjustmentDetails(string property, float value) {
            if(adjustments.ContainsKey(property))
                return adjustments[property].Details(value);
            return "";
        }

        public float AdjustProperty(string property, float value) {
            if(adjustments.ContainsKey(property)) {
                return adjustments[property].Adjust(value);
            }
            return value;
        }
    }
}