using System;
using System.IO;
using System.Net.Http;

using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;

namespace dg.unittest.api
{
    public class TestServerFixture
    {
        public IConfigurationRoot Config { get; }
        public HttpClient Client { get; }
        public TestServer _server { get; }
        public IConfigurationRoot Configuration { get; }
        public IServiceCollection Services { get; private set; }

        public TestServerFixture()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
            Configuration = builder.Build();

            var webHostBuilder = new WebHostBuilder()
                    .UseEnvironment("Testing")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseConfiguration(Configuration)
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
            Services = services;
            services.AddMvc();

            ConfigureValidation(services);
            services.AddScoped<IPeopleService>(x => new PeopleSqlService(null));
        }

        public virtual void ConfigureValidation(IServiceCollection services)
        {
            services.AddValidateInputAttribute<PersonValidator>();
        }

        public IMvcBuilder ConfigureMvcForValidateInputAttribute<T>() where T : class
        {
            return ConfigureMvcForValidateInputAttribute<T>(Services);
        }

        public IMvcBuilder ConfigureMvcForValidateInputAttribute<T>(IServiceCollection services) where T : class
        {
            return
                services.AddMvc()
                    .AddValidatorsFromAssemblyContaining<T>();
        }

        public IMvcBuilder ConfigureMvcForValidateInputFilter<T>() where T : class
        {
            return ConfigureMvcForValidateInputFilter<PersonValidator>(Services);
        }

        public IMvcBuilder ConfigureMvcForValidateInputFilter<T>(IServiceCollection services) where T : class
        {
            return
                services.AddValidateInputFilter<PersonValidator>(); 

        }

        public IServiceCollection AddScopedService<TService, TServiceImpl>() where TService : class
                                                               where TServiceImpl : class, TService
        {
           return  Services.AddScoped<TService, TServiceImpl>();
        }

        public  IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) 
            where TService : class
        {
            return Services.AddScoped(implementationFactory);
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
