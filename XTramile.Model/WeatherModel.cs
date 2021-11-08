using System;
using System.Collections.Generic;
namespace XTramile.Model
{
    public class WeatherModel
    {
        public Coords Coord { get; set; }
        public List<Weathers> Weather { get; set; }
        public string Base { get; set; }

        public Mains Main { get; set; }
        public int? Visibility { get; set; }
        public Winds Wind { get; set; }
        public Clouds Cloud { get; set; }
        public int? Dt { get; set; }
        public Syss Sys { get; set; }
        public int? Timezone { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Cod { get; set; }
        public DateTime? TimeWeather
        {
            get
            {
                var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                if (Dt == null || Dt == 0) Dt = 0;
                long unixTimeStampInTicks = (long)(Dt * TimeSpan.TicksPerSecond);
                return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
            }
        }

    }

}
