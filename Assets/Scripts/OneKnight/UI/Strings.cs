using UnityEngine;
using System.Collections;
using OneKnight.Loading;

namespace OneKnight {
    public static class Strings{


        public static string Get(string key) {
            return StringResources.Get(key);
        }

        public static string Format(string key, params object[] args) {
            return StringResources.Format(key, args);
        }

        public const string CELSIUS_UNIT = "°C";
        public const string DEGREES_UNIT = "°";

        public const string METER_UNIT = "m";
        public const string KM_UNIT = "km";
        public const string AU_UNIT = "AU";
        public const string LY_UNIT = "ly";
        public const string ACCELERATION_UNIT = "m/s^2";

        public const string SOLAR_RADIUS_UNIT = "Solar radii";
        public const string EARTH_RADIUS_UNIT = "Earth radii";

        public const string KG_UNIT = "kg";
        public const string MT_UNIT = "Megatonnes";
        public const string SOLAR_MASS_UNIT = "Solar Masses";
        public const string EARTH_MASS_UNIT = "Earth Masses";

        public const string SECOND_UNIT = "s";
        public const string MINUTE_UNIT = "minutes";
        public const string HR_UNIT = "hr";
        public const string DAY_UNIT = "Earth Days";
        public const string YEAR_UNIT = "Earth Years";

        public const string NANOTESLA_UNIT = "nT";
        public const string MICROTESLA_UNIT = "µT";
        public const string GAUSS_UNIT = "G";
        public const string TESLA_UNIT = "T";

        public const string MilliliterUnit = "mL";
        public const string LiterUnit = "L";
        public const string CubicMeterUnit = "cubic meters";

        public static readonly string[] syllables = {
            "aa", "ab", "ac", "ad", "aed", "ael", "aer", "aes", "aeth", "aex", "af", "ag", "ah", "ak", "al", "am", "an", "ap", "aph", "ar", "as", "at", "aul", "aun", "aum", "aw", "awl", "awn", "ax", "az",
            "ba", "be", "bee", "bi", "bla", "ble", "blee", "bli", "blo", "bloo", "blu", "blue", "bo", "boo", "bra", "bre", "bree", "bri", "bro", "broo", "bru", "brue", "bu", "bue", "by"
        };

        public static string Embolden(string s) {
            return "<b>" + s + "</b>";
        }

        public static string Italicize(string s) {
            return "<i>" + s + "</i>";
        }

        public static string Color(string s, string color) {
            return "<color=" + color + ">" + s + "</color>";
        }

        public static string DecimalProperty(string keyname, float value) {
            return string.Format("<b>{0}:</b> {1:f2}", Get(keyname), value);
        }


        public static string FormatMass(float massKg) {
            if(massKg < UnitConversion.MEGATONNE_THRESHOLD)
                return massKg + " " + KG_UNIT;
            if(massKg < UnitConversion.EARTH_MASS_THRESHOLD)
                return massKg * UnitConversion.KG_TO_MEGATONNE + " " + MT_UNIT;
            if(massKg < UnitConversion.SOLAR_MASS_THRESHOLD)
                return massKg * UnitConversion.KG_TO_EARTH_MASS + " " + EARTH_MASS_UNIT;
            return massKg  *UnitConversion.KG_TO_SOLAR_MASS + " " + SOLAR_MASS_UNIT;
        }

        public static string FormatRadius(float radiusKm) {
            if(radiusKm < UnitConversion.KM_RADIUS_THRESHOLD)
                return radiusKm * 1000 + " " + METER_UNIT;
            if(radiusKm < UnitConversion.EARTH_RADIUS_THRESHOLD)
                return radiusKm + " " + KM_UNIT;
            if(radiusKm < UnitConversion.SOLAR_RADIUS_THRESHOLD)
                return radiusKm * UnitConversion.KM_TO_EARTH_RADIUS + " " + EARTH_RADIUS_UNIT;
            return radiusKm * UnitConversion.KM_TO_SOLAR_RADIUS + " " + SOLAR_RADIUS_UNIT;
        }

        public static string FormatDistance(float distKm) {
            if(distKm < UnitConversion.KM_DISTANCE_THRESHOLD)
                return distKm * 1000 + " " + METER_UNIT;
            if(distKm < UnitConversion.AU_THRESHOLD)
                return distKm + " " + KM_UNIT;
            if(distKm < UnitConversion.LY_THRESHOLD)
                return distKm*UnitConversion.KM_TO_AU + " " + AU_UNIT;
            return distKm * UnitConversion.KM_TO_LY + " " + LY_UNIT;

        }

        public static string FormatTime(float timeDays) {
            if(timeDays < UnitConversion.MINUTE_TIME_THRESHOLD)
                return string.Format("{0:g3}" + " " + SECOND_UNIT, timeDays * UnitConversion.DAYS_TO_SECONDS);
            if(timeDays < UnitConversion.HOUR_TIME_THRESHOLD)
                return string.Format("{0:g3}" + " " + MINUTE_UNIT, timeDays * UnitConversion.DAYS_TO_MINUTES);
            if(timeDays < UnitConversion.DAY_TIME_THRESHOLD)
                return string.Format("{0:f1}" + " " + HR_UNIT, timeDays * 24);
            if(timeDays < UnitConversion.YEAR_TIME_THRESHOLD)
                return string.Format("{0:g3}" + " " + DAY_UNIT, timeDays);
            if(timeDays < UnitConversion.ManyYearTimeThreshold) {
                return string.Format("{0:g3}" + " " + YEAR_UNIT, timeDays / 365);
            }
            return string.Format("{0:n0}" + " " + YEAR_UNIT, timeDays / 365);
        }

        public static string FormatMagnetic(float field) {
            if(field < UnitConversion.NANO_MAGNETIC_THRESHOLD)
                return field * 1e9f + " " + NANOTESLA_UNIT;
            if(field < UnitConversion.MICRO_MAGNETIC_THRESHOLD)
                return field * 1e6f + " " + MICROTESLA_UNIT;
            if(field < UnitConversion.GAUSS_MAGNETIC_THRESHOLD)
                return field * 1e4f + " " + GAUSS_UNIT;
            else
                return field + " " + TESLA_UNIT;
        }

        public static string FormatVolume(float cubicM) {
            int tier = VolumeUnitTier(cubicM);
            return ConvertVolume(cubicM, tier) + " " + VolumeUnit(tier);
        }

        public static int VolumeUnitTier(float cubicM) {
            if(cubicM < UnitConversion.MilliliterThreshold)
                return 0;
            if(cubicM < UnitConversion.LiterThreshold)
                return 1;
            return 2;
        }

        public static float ConvertVolume(float cubicM, int tier) {
            switch(tier) {
                case 0:
                    return 1e6f*cubicM;
                case 1:
                    return 1000*cubicM;
                case 2:
                default:
                    return cubicM;
            }
        }

        public static string VolumeUnit(int tier) {
            switch(tier) {
                case 0:
                    return MilliliterUnit;
                case 1:
                    return LiterUnit;
                case 2:
                default:
                    return CubicMeterUnit;
            }
        }

        public static string GetID(int code) {
            int alpha = Mathf.Abs(code) % (26*26);
            string result = char.ToString((char)((alpha % 26) + 65)) + (char)((alpha / 26) + 65);
            result += "#" + (Mathf.Abs(code * 53)) % 1000000;
            return result;
        }

        public static string GetNumberID(int code, int digits) {
            string result = "" + Mathf.Abs(code*53) % Mathf.Pow(10, digits);
            while(result.Length < digits)
                result += "0";
            return result;
        }

        public static string AlphaID(int code, int maximum, bool upper) {
            int chars = Mathf.CeilToInt(Mathf.Log(maximum, 26));
            if(chars == 0)
                chars = 1;
            string result = "";
            code -= 1;

            int offset = (upper ? 65 : 97);
            while(chars > 0) {
                result += char.ConvertFromUtf32((code % 26) + offset);
                code /= 26;
                chars--;
            }
            return result;
        }

        public static string AlphaID(int code, int maximum) {
            return AlphaID(code, maximum, true);
        }

        public static string AlphaID(int code, bool upper) {
            return AlphaID(code, code, upper);
        }

        public static string ToRomanNumeral(int number, bool upper) {
            string result = "";
            if(upper) {
                while(number >= 1000) {
                    result += "M";
                    number -= 1000;
                }
                if(number >= 900) {
                    result += "CM";
                    number -= 900;
                }
                if(number >= 500) {
                    result += "D";
                    number -= 500;
                }
                if(number >= 400) {
                    result += "CD";
                    number -= 400;
                }
                while(number >= 100) {
                    result += "C";
                    number -= 100;
                }
                if(number >= 90) {
                    result += "XC";
                    number -= 90;
                }
                if(number >= 50) {
                    result += "L";
                    number -= 50;
                }
                if(number >= 40) {
                    result += "XL";
                    number -= 40;
                }
                while(number >= 10) {
                    result += "X";
                    number -= 10;
                }
                if(number == 9) {
                    result += "IX";
                    number -= 9;
                }
                if(number >= 5) {
                    result += "V";
                    number -= 5;
                }
                if(number == 4) {
                    result += "IV";
                    number -= 4;
                }
                while(number >= 1) {
                    result += "I";
                    number -= 1;
                }
                if(number != 0) {
                    Debug.Log("Roman numeral failed: " + number + " so far " + result);
                }
            } else {
                while(number >= 1000) {
                    result += "m";
                    number -= 1000;
                }
                if(number >= 900) {
                    result += "cm";
                    number -= 900;
                }
                if(number >= 500) {
                    result += "d";
                    number -= 500;
                }
                if(number >= 400) {
                    result += "cd";
                    number -= 400;
                }
                while(number >= 100) {
                    result += "c";
                    number -= 100;
                }
                if(number >= 90) {
                    result += "xc";
                    number -= 90;
                }
                if(number >= 50) {
                    result += "l";
                    number -= 50;
                }
                if(number >= 40) {
                    result += "xl";
                    number -= 40;
                }
                while(number >= 10) {
                    result += "x";
                    number -= 10;
                }
                if(number == 9) {
                    result += "ix";
                    number -= 9;
                }
                if(number >= 5) {
                    result += "v";
                    number -= 5;
                }
                if(number == 4) {
                    result += "iv";
                    number -= 4;
                }
                while(number >= 1) {
                    result += "i";
                    number -= 1;
                }
                if(number != 0) {
                    Debug.Log("Roman numeral failed: " + number + " so far " + result);
                }
            }
            return result;


        }

        public static string FirstLetterCap(string input) {
            return input[0] >= 97 ? char.ConvertFromUtf32(input[0]-32) + input.Substring(1) : input;
        }


        public static string ItemList(System.Collections.Generic.IEnumerable<Drop> items) {
            return ItemList(items, "\n");
        }

        public static string ItemList(System.Collections.Generic.IEnumerable<Drop> items, string delimiter) {
            string result = "";
            foreach(Drop item in items) {
                result += item + delimiter;
            }
            return result;
        }
    }
}