using XTramile.Infrastructure.IServiceProvider;
using XTramile.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using XTramile.Infrastructure.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
namespace XTramile.Infrastructure.HelperService
{
    public class XtramileApiWeatherService : IXtramileApiWeatherService
    { 
        private readonly IHttpClientFactory _clientFactory;
        private string BaseUrl {get;set;}
        private string AppId {get;set;}
        private string Units {get;set;}
        private string Lang {get;set;}
        public StatusResponse StatusResponses {get;set;} 
        public XtramileApiWeatherService(IHttpClientFactory clientFactory, IOptions<ApiSettings> configure)
        {
            _clientFactory = clientFactory;
            BaseUrl = configure.Value.BaseUrl;
            AppId = configure.Value.Token;
            Units = configure.Value.Units;
            Lang = configure.Value.Lang;
        }

        public async Task<WeatherModel> Get(string country,string city)
        {
            string urlApi = $"{BaseUrl}q={city},{country}&units={Units}&lang={Lang}&appid={AppId}";
            var request = new HttpRequestMessage(HttpMethod.Get,urlApi);
            StatusResponses = new StatusResponse();
            try{
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if(response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var dataWeather = JsonConvert.DeserializeObject<WeatherModel>(responseString);
                    StatusResponses.StatusCode = CodeResponse.Success;
                    return dataWeather;
                }
                else{
                    StatusResponses.Message = "Api not response";
                    StatusResponses.StatusCode = CodeResponse.NotFound; 
                    return new WeatherModel();
                }             
            }
            catch(HttpRequestException e)
            {
                // I just make one exception for all status <> OK/Success...just to make it simple away..
                // because we will never to know what can be change in next from third party API, 
                // it mean status code, even them notice in first time or in website,but we never to know when them change, and app stil run.. 
                // 
                StatusResponses.Message = e.Message;
                StatusResponses.StatusCode = CodeResponse.Error; 
                return new WeatherModel();
            }

        }

    }

}
