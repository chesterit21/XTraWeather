using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using XTramile.Infrastructure.Configuration;
using XTramile.Model;

namespace XTramile.XUnitTestController.Infrastructure
{
    public static class OptionsBuilder
    {
        public static IOptions<FileSettings> FileSettingOption()
        {
            return Options.Create<FileSettings>(new FileSettings{ FileCity = "FileCity.json", FileCountry = "FileCountry.csv"});
        }
        public static IOptions<ApiSettings> ApiSettingsOption()
        {
            return Options.Create<ApiSettings>(new ApiSettings{ BaseUrl = "https://apiweatheramp.org/", Token = "XTRAMILE0349",  Lang = "en", Units = "imperial"});
        }
    } 

    public static class ClientBuilder
        {
            public static IHttpClientFactory OpenWeatherClientFactory(StringContent content, HttpStatusCode statusCode = HttpStatusCode.OK)
            {
                var handler = new Mock<HttpMessageHandler>();
                handler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = statusCode,
                        Content = content
                    });
                var client = new HttpClient(handler.Object);
                var clientFactory = new Mock<IHttpClientFactory>();
                clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                    .Returns(client);
                return clientFactory.Object;
            }
        }

public static class OpenWeatherOrgResponses
    {
        public static StringContent OkResponse => BuildOkResponse();
        public static StringContent UnauthorizedResponse => BuildUnauthorizedResponse();
        public static StringContent NotFoundResponse => BuildNotFoundResponse();
        public static StringContent InternalErrorResponse => BuildInternalErrorResponse();
        private static StringContent BuildOkResponse()
        {
            var response = new WeatherModel
            {
                 Dt = 1594155600, Main = new Mains { Temp = (double)32.93 }
            };
            var json = JsonSerializer.Serialize(response);
            return new StringContent(json);
        }
        private static StringContent BuildUnauthorizedResponse()
        {
            var json = JsonSerializer.Serialize(new { Cod = 401, Message = "Invalid Api key." });
            return new StringContent(json);
        }
        private static StringContent BuildNotFoundResponse()
        {
            var json = JsonSerializer.Serialize(new { Cod = 404, Message = "city not found" });
            return new StringContent(json);
        }
        private static StringContent BuildInternalErrorResponse()
        {
            var json = JsonSerializer.Serialize(new {Cod = 500, Message = "Internal Error."});
            return new StringContent(json);
        }
    }
}