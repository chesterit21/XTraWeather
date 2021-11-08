using XTramile.Model;
using System.Threading.Tasks;
using XTramile.Infrastructure.Configuration;

namespace XTramile.Infrastructure.IServiceProvider
{
    public interface IXtramileApiWeatherService
    {
        Task<WeatherModel> Get(string country,string city);
        StatusResponse StatusResponses {get;set;}        
    }

}
