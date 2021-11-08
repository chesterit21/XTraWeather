using System;
using System.Threading.Tasks;
using XTramile.Model;
using XTramile.XUnitTestController.Infrastructure;
using Xunit;

namespace XTramile.XUnitTestController
{
    public class WeatherTestService
    {
        [Fact]
        public async Task Return_Weater_Data_FromAPI()
        {
            var opts = OptionsBuilder.ApiSettingsOption();
            var clientFactory = ClientBuilder.OpenWeatherClientFactory(OpenWeatherOrgResponses.OkResponse);
            var Xtraserice = new XTramile.Infrastructure.HelperService.XtramileApiWeatherService(clientFactory,opts);
            var result = await Xtraserice.Get("Id","Depok");
            Assert.IsType<WeatherModel>(result);
        }
    }
}
