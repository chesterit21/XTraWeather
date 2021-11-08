using System;
namespace XTramile.Model
{
    public class Mains
    {
        public double? Temp { get; set; }
        public double? Feels_Like { get; set; }
        public double? Temp_Min { get; set; }
        public double? Temp_Max { get; set; }
        public double? Pressure { get; set; }
        public int? Humidity { get; set; }
        public string DewPoint
        {
            get
            {
                var humadity = UnitsNet.RelativeHumidity.FromPercent(Convert.ToDouble(Humidity));
                var tempUnits = UnitsNet.Temperature.FromDegreesFahrenheit(Convert.ToDouble(Temp));
                var dewPoint = Iot.Device.Common.WeatherHelper.CalculateDewPoint(tempUnits, humadity);
                return dewPoint.ToString();
            }
        }
        //public string? TempCelcius {get { var tempCel = (Temp - 32)* 5/9; return tempCel;}}
        public string TempCelcius
        {
            get
            {
                var calCel = (Temp - 32) * 5 / 9;
                var tempCel = UnitsNet.Temperature.FromDegreesCelsius(Convert.ToDouble(calCel));
                return tempCel.ToString();
            }
        }
    }

}
