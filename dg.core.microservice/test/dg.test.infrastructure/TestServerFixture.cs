using System;
using System.Net.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

using Newtonsoft.Json;
using FluentValidation.Results;

using dg.common.validation;
using dg.contract;

namespace dg.test.infrastructure
{
    public class TestServerFixture
    {
        private const string ENV_KEY = "ASPNETCORE_ENVIRONMENT";
        protected string EnvironmentName;
        protected IConfigurationRoot Configuration { get; set; }
        protected TestServer Server { get; set; }
        public HttpClient Client { get; set; }

        public TestServerFixture()
        {
            SetEnvironment();
            BuildConfiguration();
            CreateTestServer(BuildWebHost());
        }

        private void SetEnvironment()
        {
            EnvironmentName = Environment.GetEnvironmentVariable(ENV_KEY);
            if (string.IsNullOrWhiteSpace(EnvironmentName))
            {
                EnvironmentName = "Local";
            }
        }

        private void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IWebHostBuilder BuildWebHost()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseKestrel()
            //  .UseContentRoot(PlatformServices.Default.Application.ApplicationBasePath)
              .ConfigureServices(ConfigureServices)
              .Configure(ConfigureApp)
              .UseEnvironment(EnvironmentName);

            return webHostBuilder;
        }

        private void CreateTestServer(IWebHostBuilder webHost)
        {
            Server = new TestServer(webHost);
            Client = Server.CreateClient();
            Client.BaseAddress = ClientBaseAddress;
        }


        protected virtual Uri ClientBaseAddress
        {
            get { return new Uri("http://localhost:5000"); }
        }

        public virtual void ConfigureApp(IApplicationBuilder app)
        {
            app.UseMvc();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(x => Configuration);
            services.AddMvc();
        }

        protected IMvcBuilder ConfigureMvcForValidateInputAttribute<T>(IServiceCollection services) where T : class
        {
            return
                services.AddMvc()
                        .AddValidatorsFromAssemblyContaining<T>();
        }

        protected IMvcBuilder ConfigureMvcForValidateInputFilter<T>(IServiceCollection services) where T : class
        {
            var mvcBuilder = ConfigureMvcForValidateInputAttribute<T>(services);
            mvcBuilder.AddActionFilterValidator<T>();
            return mvcBuilder;
        }

        public StringContent BuildRequestContent(Person person)
        {
            var json = JsonConvert.SerializeObject(person);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return content;
        }

        public ValidationResult GetValidationResult(HttpResponseMessage response)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var errorResponse = JsonConvert.DeserializeObject<ValidationResult>(json);
            return errorResponse;
        }

    }
}
