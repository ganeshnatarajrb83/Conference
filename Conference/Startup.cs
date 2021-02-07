using Conference.Domain;
using Conference.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;

namespace Conference
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string databaseName { get; set; }
        public static string containerName { get; set; }
        public static string account { get; set; }
        public static string key { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<CosmosDb>(Configuration.GetSection("CosmosDb"));
            var settings = Configuration.GetSection("CosmosDb").Get<CosmosDb>();
            services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(settings).GetAwaiter().GetResult());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conference", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(CosmosDb settings)
        {

            databaseName = settings.DatabaseName;
            containerName = settings.ContainerName;
            account = settings.Account;
            key = settings.Key;

            CosmosClient client = new CosmosClient(account, key);
            CosmosDBService cosmosDBService = new CosmosDBService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/__partitionKey");

            return cosmosDBService;
        }
    }
}
