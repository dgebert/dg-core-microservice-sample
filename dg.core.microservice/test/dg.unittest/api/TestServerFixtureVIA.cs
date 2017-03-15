using System;
using System.IO;
using System.Net.Http;
using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dg.unittest.api
{
    public class TestServerFixtureVIA
    {
        public IConfigurationRoot Config { get; }
        public HttpClient Client { get; }
        private TestServer _server;
        private IConfigurationRoot Configuration { get; }

        public TestServerFixtureVIA()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
            Configuration = builder.Build();

            var webHostBuilder = new WebHostBuilder()
                    .UseEnvironment("Testing")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseKestrel()
                   // Configure
                    .Configure(ConfigureApp)
                    .ConfigureServices(ConfigureServices);

            _server = new TestServer(webHostBuilder);
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri(@"http://localhost:5000/");
        }

        public virtual void ConfigureApp(IApplicationBuilder app)
        {
            app.UseMvc();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        //    services.AddSingleton(x => Configuration);
            services.AddMvc();
            services.AddScoped<IPeopleService>(x => new PeopleSqlService(null));

            ConfigureMvcForValidateInputAttribute<PersonValidator>(services);

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


        protected virtual IMvcBuilder ConfigureFluentValidation<T>(IServiceCollection services) where T: class
        {
            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddValidatorsFromAssemblyContaining<T>();
            return mvcBuilder;
        }

        public void Dispose()
        {
            if (Client != null)
            {
                Client.Dispose();
            }
            if (_server != null)
            {
                _server.Dispose();
            }
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
