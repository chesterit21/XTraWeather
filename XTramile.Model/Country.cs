using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
namespace XTramile.Model
{
    public class Country
    {
        private List<City> citys = new List<City>();

        [Name("country_code")]
        public string CountryCode { get; set; }

        [Name("country_name")]
        public string CountryName { get; set; }

        public List<City> Citys { get => citys; set => citys = value; }
    }

}
