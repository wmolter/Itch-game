using UnityEngine;
using System.Collections;
namespace OneKnight {
    public static class UnitConversion {

        public const float MEGATONNE_TO_KG = 1000000f;
        public const float KG_TO_MEGATONNE = 1/MEGATONNE_TO_KG;
        public const float SOLAR_MASS_TO_KG = 1.98855E30f;
        public const float KG_TO_SOLAR_MASS = 1/SOLAR_MASS_TO_KG;
        public const float EARTH_MASS_TO_KG = 5.9722E24f;
        public const float KG_TO_EARTH_MASS = 1/EARTH_MASS_TO_KG;
        public const float JUPITER_MASS_TO_KG = 1.8982E27f;
        public const float KG_TO_JUPITER_MASS = 1/JUPITER_MASS_TO_KG;
        public const float SOLAR_MASS_TO_EARTH_MASS = 332946f;
        public const float EARTH_MASS_TO_SOLAR_MASS = 1/SOLAR_MASS_TO_EARTH_MASS;
        public const float MEGATONNE_THRESHOLD = 1.0f * MEGATONNE_TO_KG;
        public const float EARTH_MASS_THRESHOLD = .00001f * EARTH_MASS_TO_KG;
        public const float SOLAR_MASS_THRESHOLD = .01f * SOLAR_MASS_TO_KG;

        public const float SOLAR_RADIUS_TO_KM = 6.957E5f;
        public const float KM_TO_SOLAR_RADIUS = 1/SOLAR_RADIUS_TO_KM;
        public const float EARTH_RADIUS_TO_KM = 6371f;
        public const float KM_TO_EARTH_RADIUS = 1/EARTH_RADIUS_TO_KM;
        public const float SOLAR_RADIUS_TO_EARTH_RADIUS = SOLAR_RADIUS_TO_KM/EARTH_RADIUS_TO_KM;
        public const float KM_RADIUS_THRESHOLD = 10f;
        public const float EARTH_RADIUS_THRESHOLD = .1f*EARTH_RADIUS_TO_KM;
        public const float SOLAR_RADIUS_THRESHOLD = .1f*SOLAR_RADIUS_TO_KM;

        public const float AU_TO_KM = 149597870.7f;
        public const float KM_TO_AU = 1/AU_TO_KM;
        public const float LY_TO_KM = 9460730472580.8f;
        public const float KM_TO_LY = 1/LY_TO_KM;
        public const float LY_TO_AU = 63241.077f;
        public const float AU_TO_LY = 1/LY_TO_AU;
        public const float KM_DISTANCE_THRESHOLD = .5f;
        public const float AU_THRESHOLD = .05f * AU_TO_KM;
        public const float LY_THRESHOLD = .05f * LY_TO_KM;

        public const float DAYS_TO_SECONDS = 24*3600;
        public const float DAYS_TO_MINUTES = 24*60;
        public const float SECONDS_TO_MINUTES = 1/60f;
        public const float SECONDS_TO_DAYS = 1/DAYS_TO_SECONDS;
        public const float MINUTES_TO_DAYS = 1/DAYS_TO_MINUTES;
        public const float MINUTE_TIME_THRESHOLD = MINUTES_TO_DAYS*2;
        public const float HOUR_TIME_THRESHOLD = 1f/12;
        public const float DAY_TIME_THRESHOLD = 3;
        public const float YEAR_TIME_THRESHOLD = 365*2;
        public const float ManyYearTimeThreshold = 365*1000;

        public const float NANO_MAGNETIC_THRESHOLD = 1e-7f;
        public const float MICRO_MAGNETIC_THRESHOLD = 1e-5f;
        public const float GAUSS_MAGNETIC_THRESHOLD = 1e-2f;

        public const float CUBIC_METERS_TO_LITERS = 1000;
        public const float LITERS_TO_CUBIC_METERS = 1e-3f;

        public const float MilliliterThreshold = 0.001f;
        public const float LiterThreshold = .25f;

        public static System.TimeSpan DaysToSpan(float timeDays) {
            float time = timeDays;
            int d = (int)time;
            time = (time-d)*24;
            int h = (int)time;
            time = (time-h)*60;
            int m = (int)time;
            time = (time-m)*60;
            int s = (int)time;
            return new System.TimeSpan(d, h, m, s);
        }
    }

}