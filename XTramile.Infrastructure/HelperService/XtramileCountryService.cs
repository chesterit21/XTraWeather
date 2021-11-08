using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTramile.Infrastructure.IServiceProvider;
using Microsoft.Extensions.Options;
using XTramile.Infrastructure.Configuration;
using XTramile.Model;

namespace XTramile.Infrastructure.HelperService
{
    public class XtramileCountryService : IXtramileCountryService
    {
        //for documentation implement CsvHelper new Update...
        //https://joshclose.github.io/CsvHelper/getting-started/#reading-a-csv-file

        //getting file list of city from this url :
        //http://bulk.openweathermap.org/sample/


        private readonly IXtramileCacheService<Country> _cache;

        private string PathFileCountry { get; set; }
        private string CacheCodeCountry { get; set; }
        public StatusFile StatusFileInfo {get;set;}
        public XtramileCountryService(IOptions<FileSettings> configure, IXtramileCacheService<Country> cache)
        {
            PathFileCountry = configure.Value.FileCountry;
            CacheCodeCountry = typeof(Country).Name; // make code cache from type name...
            _cache = cache;
        }
        public async Task<IEnumerable<Country>> GetListCountry(string path)
        {
            var listCountry = new List<Country>();
            listCountry = _cache.TryGetValue(CacheCodeCountry); // try get from cache...
            if (listCountry.Count != 0) return await Task.Run(() => listCountry); // if ready, return, dont need read a file again...
            if (File.Exists(Path.Combine(path, PathFileCountry)))
            {
                using var reader = new StreamReader(Path.Combine(path, PathFileCountry), Encoding.Default);
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                listCountry = csv.GetRecords<Country>().ToList();
                if(listCountry != null || listCountry.Count != 0) 
                {
                    _cache.CreateEntry(CacheCodeCountry, listCountry);
                    StatusFileInfo = StatusFile.ReadyForConsume;
                }
                else{
                    StatusFileInfo = StatusFile.FileDataIsEmpty;
                }
                return await Task.Run(() => listCountry);
            }
            else
            {
                StatusFileInfo = StatusFile.NotExists;
                return await Task.Run(() => listCountry);
            }
        }

    }

}
