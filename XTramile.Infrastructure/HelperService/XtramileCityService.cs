using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XTramile.Infrastructure.IServiceProvider;
using Microsoft.Extensions.Options;
using XTramile.Model;
using XTramile.Infrastructure.Configuration;
namespace XTramile.Infrastructure.HelperService
{
    public class XtramileCityService : IXtramileCityService
    {
        //getting file list of city from this url :
        //http://bulk.openweathermap.org/sample/

        private readonly IXtramileCacheService<City> _cache;
        private string PathFileCity {get;set;}
        private string CacheCodeCity {get;set;}     
        public StatusFile StatusFileInfo {get;set;}
        public XtramileCityService(IOptions<FileSettings> configure, IXtramileCacheService<City> cache) {
            PathFileCity = configure.Value.FileCity;
            CacheCodeCity = typeof(City).Name;
            _cache = cache;
         }

        public async Task<IEnumerable<City>> GetListCity(string path,string country)
        {
            var listCity = new List<City>();
            listCity= _cache.TryGetValue(CacheCodeCity); // try get from cache...
            if(listCity.Count != 0) return await Task.Run(() => listCity.Where(c => c.Country?.ToLower().Trim() == country?.ToLower().Trim()).ToList()); // if ready, return, dont need read a file again...
           
            if (File.Exists(Path.Combine(path, PathFileCity)))
            {
                var stringCity = File.ReadAllText(Path.Combine(path,PathFileCity));
                listCity = JsonConvert.DeserializeObject<List<City>>(stringCity);

                if (listCity != null || listCity.Count != 0)
                {
                    _cache.CreateEntry(CacheCodeCity,listCity);
                    listCity = listCity
                                        .Where(c => c.Country?.ToLower().Trim() == country?.ToLower().Trim())
                                        .ToList();
                    StatusFileInfo = StatusFile.ReadyForConsume;
                }
                else {
                    StatusFileInfo = StatusFile.FileDataIsEmpty;
                }
                return await Task.Run(() => listCity);
            }
            else {
                StatusFileInfo = StatusFile.NotExists;
                return await Task.Run(() => listCity);
            }

        }
    }

}
