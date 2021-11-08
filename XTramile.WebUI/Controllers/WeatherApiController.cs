using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XTramile.Infrastructure.IServiceProvider;
using Microsoft.AspNetCore.Hosting;
using System;

namespace XTramile.WebUI.Controllers
{
    [Route("weather")]
    [ApiController]
    public class WeatherApiController :Controller
    {
        private readonly IXtramileCountryService _countryService;
        private readonly IXtramileCityService _cityService;
        private readonly IXtramileApiWeatherService _apiService;
        private readonly IWebHostEnvironment _env;
        public WeatherApiController(IXtramileCountryService countryService,IXtramileCityService cityService, IXtramileApiWeatherService apiService,IWebHostEnvironment env)
        {
            _countryService = countryService;
            _cityService = cityService;
            _apiService = apiService;
            _env = env;
        }

        [Route("country")]
        public async Task<IActionResult> GetCountry()
        {
            var path = _env.ContentRootPath;
            var listCountry = await _countryService.GetListCountry(path); 
            if(_countryService.StatusFileInfo == Infrastructure.Configuration.StatusFile.FileDataIsEmpty) {
                Response.StatusCode = 404;
                return Json("service is cannot getting data country, maybe data is empty or not found");
            } else if(_countryService.StatusFileInfo == Infrastructure.Configuration.StatusFile.NotExists) {
                Response.StatusCode = 500;
                return Json("File data not found, or not ready in path.");
            }

            return Json(listCountry);
        }

        [Route("city/{country}")]
        public async Task<IActionResult> GetCity(string country)
        {
            var path = _env.ContentRootPath;
            var listCity = await _cityService.GetListCity(path,country); 
            if(_countryService.StatusFileInfo == Infrastructure.Configuration.StatusFile.FileDataIsEmpty) {
                Response.StatusCode = 404;
                return Json("service is cannot getting data country, maybe data is empty or not found");
            } else if(_countryService.StatusFileInfo == Infrastructure.Configuration.StatusFile.NotExists) {
                Response.StatusCode = 500;
                return Json("File data not found, or not ready in path.");
            }
            return Json(listCity);
        }

        [Route("get/{country?}/{city?}")]
        public async Task<JsonResult> GetWeather(string country,string city)
        {
            var data = await _apiService.Get(country,city);
            if(_apiService.StatusResponses.StatusCode == Infrastructure.Configuration.CodeResponse.Error) {
                Response.StatusCode = 500;
                return Json("File data not found, or not ready in path.");
            } else if(_apiService.StatusResponses.StatusCode == Infrastructure.Configuration.CodeResponse.NotFound) {
                Response.StatusCode = 404;
                return Json("weather not found with that query, please check the query or api end poin.");
            }
            return Json(data);
        }
    }
}
