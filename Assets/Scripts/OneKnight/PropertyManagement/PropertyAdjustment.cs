using UnityEngine;
using System.Collections;

namespace OneKnight.PropertyManagement {
    [System.Serializable]
    public class PropertyAdjustment : System.IFormattable{

        public static ConditionType ConditionFromString(string s) {
            if(s == "always")
                return ConditionType.Always;
            if(s == "less")
                return ConditionType.Less;
            if(s == "greater")
                return ConditionType.Greater;
            Debug.LogWarning("Unknown condition type: " + s);
            return ConditionType.Always;
        }
        
        public enum Type {
            Modifier, Bonus, Min, Max
        }

        public enum ConditionType {
            Always, Less, Greater
        }

        struct Condition {
            public float value;
            public ConditionType type;
        }
        public Type type { get; private set; }
        public float adjustment { get; private set; }
        public string sourceID { get; private set; }
        public string property { get; private set; }

        //the base value must satisfy this condition
        Condition basecondition;
        //calculated value so far must satisfy this condition
        Condition precondition;
        //apply the adjustment, but only up to the postcondition, or don't apply at all.
        Condition postcondition;

        public PropertyAdjustment(string property, Type type, float adjustment, string sourceID) {
            this.type = type;
            this.adjustment = adjustment;
            this.sourceID = sourceID;
            this.property = property;
        }

        public void SetBaseCondition(float value, ConditionType type) {
            basecondition = new Condition();
            basecondition.value = value;
            basecondition.type = type;
        }

        public void SetPreCondition(float value, ConditionType type) {
            precondition = new Condition();
            precondition.value = value;
            precondition.type = type;
        }

        public void SetPostCondition(float value, ConditionType type) {
            postcondition = new Condition();
            postcondition.value = value;
            postcondition.type = type;
        }

        public float Apply(float soFar, float baseValue) {
            switch(basecondition.type) {
                case ConditionType.Less:
                    if(baseValue >= basecondition.value)
                        return soFar;
                    break;
                case ConditionType.Greater:
                    if(baseValue <= basecondition.value)
                        return soFar;
                    break;
                case ConditionType.Always:
                    break;
            }
            switch(precondition.type) {
                case ConditionType.Less:
                    if(soFar >= precondition.value)
                        return soFar;
                    break;
                case ConditionType.Greater:
                    if(soFar <= precondition.value)
                        return soFar;
                    break;
                case ConditionType.Always:
                    break;
            }
            float tentative = DoOperation(soFar);
            switch(postcondition.type) {
                case ConditionType.Less:
                    if(tentative < postcondition.value)
                        return tentative;
                    else
                        return Mathf.Max(postcondition.value, soFar);
                case ConditionType.Greater:
                    if(tentative > postcondition.value)
                        return tentative;
                    else
                        return Mathf.Min(postcondition.value, soFar);
                case ConditionType.Always:
                    return tentative;
                default:
                    return tentative;
            }
        }

        private bool Applies(float soFar, float baseValue) {
            switch(basecondition.type) {
                case ConditionType.Less:
                    if(baseValue >= basecondition.value)
                        return false;
                    break;
                case ConditionType.Greater:
                    if(baseValue <= basecondition.value)
                        return false;
                    break;
                case ConditionType.Always:
                    break;
            }
            switch(precondition.type) {
                case ConditionType.Less:
                    if(soFar >= precondition.value)
                        return false;
                    break;
                case ConditionType.Greater:
                    if(soFar <= precondition.value)
                        return false;
                    break;
                case ConditionType.Always:
                    break;
            }
            float tentative = DoOperation(soFar);
            switch(postcondition.type) {
                case ConditionType.Less:
                    if(tentative < postcondition.value)
                        return true;
                    else if(postcondition.value > soFar)
                        return true;
                    else
                        return false;
                case ConditionType.Greater:
                    if(tentative > postcondition.value)
                        return true;
                    else if(postcondition.value < soFar)
                        return true;
                    else
                        return false;
                case ConditionType.Always:
                    return true;
                default:
                    return true;
            }
        }

        private float DoOperation(float value) {
            switch(type) {
                case Type.Bonus:
                    return value + adjustment;
                case Type.Modifier:
                    return value * adjustment;
                //Yes, these are supposed to be opposite.
                case Type.Min:
                    return Mathf.Max(value, adjustment);
                case Type.Max:
                    return Mathf.Min(value, adjustment);
                default:
                    return value;
            }
        }

        public override string ToString() {
            return adjustment.ToString();
        }

        public string ToString(string format, System.IFormatProvider formatter) {
            return adjustment.ToString(format, formatter);
        }

        public string Details(float soFar, float baseValue) {
            if(!Applies(soFar, baseValue))
                return "";
            string result = "";
            switch(type) {
                case Type.Bonus:
                    if(adjustment > 0)
                        result += "+ " + adjustment;
                    else
                        result += "- " + -adjustment;
                    break;
                case Type.Modifier:
                    result += "* " + adjustment;
                    break;
                case Type.Min:
                    result += Strings.Get("Min") + " " + adjustment;
                    break;
                case Type.Max:
                    result += Strings.Get("Max") + " " + adjustment;
                    break;
                default:
                    result += adjustment.ToString();
                    break;
            }
            result += " (" + Strings.Get(sourceID) + ")";
            return result;
        }

        public static bool operator ==(PropertyAdjustment a, PropertyAdjustment b) {
            return a.sourceID == b.sourceID && a.property == b.property;
        }


        public static bool operator !=(PropertyAdjustment a, PropertyAdjustment b) {
            return a.sourceID != b.sourceID || a.property != b.property;
        }

        public override bool Equals(object obj) {
            if(!(obj is PropertyAdjustment))
                return false;
            return (PropertyAdjustment)obj == this;
        }

        public override int GetHashCode() {
            return sourceID.GetHashCode();
        }
    }
}