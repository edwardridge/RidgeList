using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RidgeList.Domain;
using RidgeList.Postgres;
using Google.Cloud.SecretManager.V1;
using System.Linq;
using Honeycomb.OpenTelemetry;
using Marten;
using MediatR;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RidgeList.FrontEnd.SignalRHubs;
using RidgeList.Models;
using RidgeList.ApplicationServices;

namespace RidgeList.FrontEnd
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
            var aspnetCoreEnvironment = Configuration["ASPNETCORE_ENVIRONMENT"];
            if (string.IsNullOrEmpty(aspnetCoreEnvironment))
            {
                throw new ApplicationException("ASPNETCORE_ENVIRONMENT is empty");
            }
            else
            {
                Console.WriteLine("ASPNETCORE_ENVIRONMENT: " + aspnetCoreEnvironment);
            }
            
            services.AddControllersWithViews();
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            services.AddSingleton(new Func<IServiceProvider, IDocumentStore>(s => {
                var dbSettingsFromSecrets = new DbSettings()
                {
                    DbUsername = Configuration["DbUsername"],
                    DbPassword = Configuration["DbPassword"],
                    DbHost = Configuration["DbHost"],
                    DbDatabase = Configuration["DbDatabase"]
                };
                var useSsl = Configuration["ASPNETCORE_ENVIRONMENT"] != "Development";
                var connectionString = MartenDbRepository.BuildConnectionString(dbSettingsFromSecrets, useSsl);
                return DocumentStore.For((storeOptions) =>
                {
                    storeOptions.Connection(connectionString);
                    storeOptions.DatabaseSchemaName = aspnetCoreEnvironment;
                });
            }));
            services.AddScoped<IWishlistRepository, MartenDbRepository>();
            services.AddScoped<IUserRepository, MartenDbSummaryRepository>();
            services.AddScoped<IUpdateWishlistHub, UpdateWishlistHub>();
            services.AddScoped<WishlistMapper, WishlistMapper>();
            services.AddScoped<WishlistCloner, WishlistCloner>();

            // services.AddSingleton<IWishlistRepository>(new Func<IServiceProvider, IWishlistRepository>(s => new InMemoryWishlistRepository()));

            services.AddSwaggerDocument();
            
            services.AddMediatR(typeof(Startup), typeof(Wishlist));
            
            services.AddSignalR();
            
            services.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("RidgeList"))
                    .AddSource("RidgeList")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    ///.AddJaegerExporter(options => options.Endpoint = new Uri("http://localhost:14268"))
                    .AddHoneycomb(new HoneycombOptions() 
                    {
                        ServiceName = "ridgelist",
                        ApiKey = "16764533377b6f87bc5c9c9c740a4c28",
                        Dataset = "ridgelist"
                    })
            );
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseOpenApi();
            app.UseSwaggerUi3(); 
            // app.UseReDoc(); 
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                
                endpoints.MapHub<WishlistHub>("/wishlisthub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
        }
    }
}