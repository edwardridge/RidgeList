using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RidgeList.Postgres;

namespace RidgeList.FrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                var isDev = hostContext.HostingEnvironment.IsDevelopment();
                if (!isDev)
                {
                    var config = builder.Build();
                    var projectId = config["GoogleProject"];
                    var secretName = config["GoogleSecretName"];
                    var dbSettings = GetDbSettingsFromSecrets(projectId, secretName);
                    builder.AddInMemoryCollection(new Dictionary<string, string>() 
                    {
                        { nameof(dbSettings.DbDatabase), dbSettings.DbDatabase },
                        { nameof(dbSettings.DbUsername), dbSettings.DbUsername },
                        { nameof(dbSettings.DbPassword), dbSettings.DbPassword },
                        { nameof(dbSettings.DbHost), dbSettings.DbHost },
                    });
                }
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });


        public static DbSettings GetDbSettingsFromSecrets(string projectId, string secretName)
        {
            var client = SecretManagerServiceClient.Create();

            var secretValue = client.AccessSecretVersion(new AccessSecretVersionRequest()
            {
                SecretVersionName = new SecretVersionName(projectId, secretName, "latest")
            });

            var data = secretValue.Payload.Data.ToStringUtf8();
            var dbSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<DbSettings>(data);
            return dbSettings;
        }
    }
}