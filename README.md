# XTraWeather
Consume Api weather from openweathermap.org

I am split the project in Solution to 3 part Project.
1. Model
2. Infrastructure 
3. WebApp UI Asp.Net Core
and Last In Folder Solution ther is Unist Test Project on XUnit Project initial.

Configure appSetting :
```json
"ApiSettings": {
    "BaseUrl": "https://api.openweathermap.org/data/2.5/weather?",
    "Token": "TOKEn-API",
    "Units" : "imperial",
    "Lang" : "en"
  },
  "FileSettings": {
    "FileCity": "FileProvider/citylist.json",
    "FileCountry": "FileProvider/IP2LOCATION-COUNTRY-INFORMATION.CSV"
  },
```

Configure Startup.cs
```csharp
    services.Configure<FileSettings>(options=> Configuration.GetSection("FileSettings").Bind(options));
    services.Configure<ApiSettings>(options=> Configuration.GetSection("ApiSettings").Bind(options));
```
Other Register service
```js
 // register http client for asp.net core...
            services.AddHttpClient();

            // register Memory Cache.........
            services.AddMemoryCache();

            // Register DI service...
            services.AddSingleton(typeof(IXtramileCacheService<>),typeof(XtramileCacheService<>));
            services.AddSingleton<IXtramileCountryService,XtramileCountryService>();
            services.AddSingleton<IXtramileCityService,XtramileCityService>();
            services.AddTransient<IXtramileApiWeatherService,XtramileApiWeatherService>();

            services.AddControllersWithViews().AddNewtonsoftJson(option => {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add (new JsonStringEnumConverter ());
            });

            // Compress Data Transfered...for get best performance or reduce data size transfered...
            // Compressed using default library from microsoft/..
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    "text/plain",
                    "text/html",
                    "text/css",
                    "font/woff2",
                    "application/javascript",
                    "image/x-icon",
                    "image/png"
                };

                options.EnableForHttps = true;
                options.MimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });
```

ON Project Infrastructure = Service Provider Application Place..

# In OpenweatherApi there is no API for get List By Country and No API For Get List City By Country.
So the Idea is, From file json. OpenWeather give list current City for Download...
The Data in Json for City, very huge, in Size closely 50MB, there is many more 50000 > line.
And Form Get List Country , I used File Read File CSV - The Data Country is Standard ISO.

```c#
    public interface IXtramileCountryService
    {
        Task<IEnumerable<Country>> GetListCountry(string path);
        StatusFile StatusFileInfo {get;set;} // Actualy better in one service, cause share with CityService..but it is ok lah ya.
    }
		
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
```

## IHttpClientFactory in Service 
```c#
    public interface IXtramileApiWeatherService
    {
        Task<WeatherModel> Get(string country,string city);
        StatusResponse StatusResponses {get;set;}        
    }

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

		
```
