using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;
using FluentValidation.Results;

namespace dg.unittest
{
    public class WebHostTestFixture
    {
        public IConfigurationRoot Config { get; }
        public HttpClient Client { get; }
        private TestServer _server;

        public WebHostTestFixture()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<Startup>()
              .UseEnvironment("Testing")
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseKestrel()

              // Configure
              .Configure(app => app.UseMvc())

              // Configure services - data service, fluentvalidation, validators
              .ConfigureServices(s => s.AddScoped<IPeopleService>(x => new PeopleSqlService(null)))
              .ConfigureServices(s => ConfigureFluentValidation<PersonValidator>(s))
              ;

            _server = new TestServer(webHostBuilder);
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri(@"http://localhost:5000/");
        }

        public virtual IWebHostBuilder ConfigureServices(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.Configure(app => app.UseMvc());
            return webHostBuilder;
        }


        protected virtual IMvcBuilder ConfigureFluentValidation<T>(IServiceCollection services) where T : class
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

        class Startup
        {
            public Startup(IHostingEnvironment env)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
            }

            public IConfigurationRoot Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                app.UseMvc();
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

