using System;
using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using gwn.common.validation;
using gwn.dataservice;
using gwn.entityframework.models;
using gwn.validation;


namespace gwn.api.integrationtest
{
    public class TestServerFixture: IDisposable
    {
        public const string ConnStringKey = "ConnectionStrings:DefaultConnection";
        private readonly string _connectionString;

        public HttpClient Client { get; }
        public TestServer Server { get; }
        public IConfigurationRoot Configuration { get; }
 //       public DbContextOptions<PeopleContext> DbContextOptions { get;  }
 
        public TestServerFixture()
        {
            var builder = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddEnvironmentVariables();
            Configuration = builder.Build();

            _connectionString = Configuration[ConnStringKey];
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("Cannot find connection string - check appsettings or where it is (not) located");
            }

            //var webHostBuilder = new WebHostBuilder()
            //        .UseEnvironment("Testing")
            //        .UseContentRoot(Directory.GetCurrentDirectory())
            //        .UseConfiguration(Configuration)
            //        .UseKestrel()
            //        .Configure(app => app.UseMvc())
            //        .ConfigureServices(ConfigureServices);
            //Server = new TestServer(webHostBuilder);
            //Client = Server.CreateClient();


            Server = new TestServer(new WebHostBuilder()
                         .UseStartup<Startup>());
            Client = Server.CreateClient();

            TestDbConnection();
        }
       
        private void TestDbConnection()
        {
            try
            {
                using (var db = GetDb())
                {
                    db.Database.GetDbConnection().Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid connection string? - check appSettings and your target db");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddValidateInputAttribute<PersonValidator>();
            services.AddDbContext<PeopleContext>(options => options.UseSqlServer(_connectionString));     
            services.AddScoped<IPeopleService, PeopleSqlService>();
        }

        public void Dispose()
        {
            Client?.Dispose();
            Server?.Dispose();
        }


        public StringContent BuildRequestContent(contract.Person person)
        {
            var json = JsonConvert.SerializeObject(person);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return content;
        }

        public PeopleContext GetDb()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PeopleContext>()
                              .UseSqlServer(_connectionString)
                              .Options;
            return new PeopleContext(dbContextOptions);
        }

        internal void SetUpDb()
        {
            TearDownDb();
        }
        
        internal void TearDownDb()
        {
            using (var db = GetDb())
            {
                db.Person.RemoveRange(db.Person);
                db.SaveChanges();
            }
        }
    }
}
