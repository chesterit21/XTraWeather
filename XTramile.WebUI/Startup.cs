using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using XTramile.Infrastructure.Configuration;
using XTramile.Infrastructure.HelperService;
using XTramile.Infrastructure.IServiceProvider;
namespace XTramile.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FileSettings>(options=> Configuration.GetSection("FileSettings").Bind(options));
            services.Configure<ApiSettings>(options=> Configuration.GetSection("ApiSettings").Bind(options));

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
                    // General
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
            services.AddResponseCompression(options =>
            {     IEnumerable<string> MimeTypes = new[]
                {
                    // General
                    "text/plain",
                    "text/html",
                    "text/css",
                    "font/woff2",
                    "application/javascript",
                    "image/x-icon",
                    "image/png"
                };

                options.EnableForHttps = true;
                options.ExcludedMimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseResponseCompression();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
