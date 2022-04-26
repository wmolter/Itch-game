using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.PropertyManagement {
    [System.Serializable]
    public class PropertyManager {

        private Dictionary<string, PropertyAdjuster> adjustments;

        public PropertyManager() {
            adjustments = new Dictionary<string, PropertyAdjuster>();
        }

        public PropertyAdjuster Get(string property) {
            if(HasAdjustment(property))
                return adjustments[property];
            return null;
        }

        public PropertyAdjuster GetCreate(string property) {
            if(HasAdjustment(property))
                return adjustments[property];
            PropertyAdjuster newAdjust = new PropertyAdjuster(property);
            adjustments[property] = newAdjust;
            return newAdjust;
        }

        public void AddAdjustment(PropertyAdjustment adjustment) {
            if(!adjustments.ContainsKey(adjustment.property))
                adjustments[adjustment.property] = new PropertyAdjuster(adjustment.property);
            adjustments[adjustment.property].AddAdjustment(adjustment);
        }

        public void AddRange(IEnumerable<PropertyAdjustment> toAdd) {
            foreach(PropertyAdjustment ad in toAdd) {
                AddAdjustment(ad);
            }
        }

        public void AddModifier(string property, float modifier, string sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddModifier(modifier, sourceID);
        }

        public void AddBonus(string property, float bonus, string sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddBonus(bonus, sourceID);
        }

        public void AddMin(string property, float min, string sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddMin(min, sourceID);
        }

        public void AddMax(string property, float max, string sourceID) {
            if(!adjustments.ContainsKey(property))
                adjustments[property] = new PropertyAdjuster(property);
            adjustments[property].AddMax(max, sourceID);
        }

        public void RemoveAdjustment(string property, string sourceID) {
            adjustments[property].RemoveAdjustment(sourceID);
        }

        public void RemoveAdjustment(PropertyAdjustment adjustment) {
            adjustments[adjustment.property].RemoveAdjustment(adjustment.sourceID);
        }

        public void RemoveRange(IEnumerable<PropertyAdjustment> toRemove) {
            foreach(PropertyAdjustment ad in toRemove) {
                RemoveAdjustment(ad);
            }
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