# Microservice Sample in ASP.NET Core

## Introduction

This is a sample demonstration of a simple microservice developed with MIcrosoft's  ASP.NET Core technology stack. It features many components and features of the ASP.NET Core platform.


##  API Deveopment Prerequisites

There is a vast amount of technology resources online. The following is a short list of prerequisites with links to recommended documentation for getting started:

- [Choosing .NET Core](https://docs.microsoft.com/en-us/dotnet/articles/standard/choosing-core-framework-server)
- [.NET Core](https://msdn.microsoft.com/en-us/library/dn878908(v=vs.110).aspx)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/ "ASP.NET Core is a new open-source and cross-platform framework for building modern cloud based internet connected applications, such as web apps, IoT apps and mobile backends")
- [.NET Core on github](https://dotnet.github.io/)
- [EF Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro "Core Entity Framework")
- [Swagger](http://swagger.io/)
- [Open API](https://github.com/OAI/OpenAPI-Specification)
- [Microservices](https://martinfowler.com/articles/microservices.html "Microservices overview by Martin Fowler")

## Getting Started

Here is a great place to start: [.NET Core on github](https://dotnet.github.io/)

### 1.  Install Visual Studio 2015 or 2017
>Note:  .NET Core development differs considerably between these two IDE's.  Visual Studio 2017 contains several new and improved features. The following resources best explain all this:
- [What's new in Visual Studio 2017](https://www.visualstudio.com/vs/whatsnew/)
- [Visual Studio 2017](https://www.visualstudio.com/en-us/news/releasenotes/vs2017-relnotes)  Release Notes
- [Visual Studio 2017 RC review: a look at what’s new and improved](https://raygun.com/blog/2016/12/visual-studio-2017-rc-review/)

If you install Visual Studio 2017, all .NET Core components are included

### 2. If you are running Visual Studio 2015 on Windows, install [.NET Core 1.1.1 SDK](https://www.microsoft.com/net/core#windowscmd)

### 3. If you're adventurous, check out these IDE's:
- [Visual Studio Code](https://code.visualstudio.com/)
- [JetBrains Rider](https://www.jetbrains.com/rider/download/)

## Microservice Architecture

![](./CoreWebApiArchitecture.PNG?raw=true "Microsservice Web API Architecture") 

### Example: People Service 
The sample microservice implementant is a people service which provides CRUD access to a backend People db.  A data service encapsulates the different db data access implementations. Currently there are two DB implementations: SQL Server and Document DB (no SQL). Other target stores - queues, files, message bus, etc - could also be implemented and abstracted with this data service layer.


### API Operations

| Operation | Route | Description | Request Body | Response Body | Status Codes
| --- | --- | --- | --- | --- | ---
| GET | /people | Get all people | None | List of Person contracts | 200, 404, 500
| GET | /people/{id} | Get a person by id | None | Person contract | 200, 404, 500
| POST | /people | Add a person  | Person contract | Person contract | 201, 400, 500
| PUT | /people | Update an existing  person | Person Contract | None | 201, 400, 404, 500
| DELETE | /people/{id} | Delete a person by Id | None | None | 200, 404 ,500

## Swagger generated docs

![](./Swagger.PNG?raw=true "Swagger provides a way to describe API - JSON outlines the names, order, and other details of API.") 

### Projects/Assemblies

| Project | Description | 
| --- | --- | ---
| src | |
| `dg.api` | Web API project - PeopleController provides endpoints and methods for Person CRUD operations| 
| `dg.common.exceptionhandling`  | Exception handling framework components |
| `dg.common.logging`  | Logging components |
| `dg.common.validation`  | Api contract validation - FluentValidation extensions and action filters to execute validation  |
| `dg.contract` | Contracts for web api operations   |       
| `dg.dataservice` | Data access with EF Core    |       
| `dg.document.db` | Data access with Document DB sdk  |       
| `dg.repository` | EF DB context with POCO models - generated from SQL database   |                                 
| `dg.sqldatabase` | SQL database containing People DB schema and seed scripts|  
| `dg.validator` | FluentValidation validator for Person contract |  
| test | |
| `dg.api.integrationtest` | end-to-end Integration testing for PersonController, using TestServer to host Web API | 
| `dg.unittest` | Unit testing for api controller, common, dataservice, validator |  
| `dg.test.infrastructur` | coommon components for testing - Test Fixture, Http utilities, Mocks, etc |  

### Middleware

I believe the power of .NET Core is its implementation and plugability of middleware - which is a fundamental concept for building a web server. Middleware is esentially everything that sits in between the server HTTP pipe and your application proper, executing actions in your controllers in the case of MVC. Middleware is composed of simple modular blocks that are each handed the HTTP request in turn, process it in some way, and then either return a response directly or hands off to the next block. In this way you can build easily composable blocks that provide a different part of the process. For example, one block could check for authentication credentials, another could simple log the request somewhere etc

In ASP.NET Core,  middleware is defined in `Startup.cs` in the `ConfigureServices(IServiceCollection)` and `Configure(IApplicationBuilder, IHostingEnvironment, ILoggerFactory)` methods. The pattern for adding modules is throught the use of extension methods on `IServiceCollection` and `IApplicationBuilder`.  Each of the extension methods called on the `IApplicationBuilder` adds another module to the pipeline. Here is where you plugin middleware components and register services with .NET Core's dependency injection infrastructure.

Eg. `Startup.cs`
```csharp
     // This method gets called by the runtime. Use this method to register services in the DI container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Add framework services.
        services
            .AddMvc(config =>
            {
                config.Filters.Add(typeof(ApiExceptionFilter));
            })

            //  Implicit validaton - global action filter for every request. Requires no decoration of Controller Methods
        //      .AddActionFilterValidator<PersonValidator>()    

           // ImExplicit validaton. Requires decorating Controller methods with [ValidateInput]
            .AddValidatorsFromAssemblyContaining<PersonValidator>();  

        // Register db and data service 
        services.AddDbContext<PeopleContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
        services.AddScoped<IPeopleService>(x => new PeopleSqlService(x.GetService<PeopleContext>()));

        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Info { Title = "People API", Version = "v1" });
        });
    }
         

    // This method gets called by the runtime. Use this method to configure middleware for the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        loggerFactory.AddDebug();

        app.UseMvc();

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "People API");
        });
    }


```


## Logging

Nlog with targets for:
- File
- Console
- DB
- Application Insights
- Serilog (?)

## Exception Handling
- Global exception handling 

## Validation

Input validation uses `FluentValidation.AspNetCore` along with action filters and extensions to plug in filters into the request pipeline. `FluentValidation` is a popular open source validation library for .NET that uses fluent interface and lambda expressions for building validation rules for model objects. It also is an example of good compoenent design because it does one thing well, is extensible, and can be tested in isoldation. For more info, go to [FluentValidation](https://github.com/JeremySkinner/FluentValidation) site on gihub.

### Plugging into Request pipeline

Recall in `Startup.cs` where we register filter for FluentValidation.  

The package `dg.validation` adds extensions to plugin FluentValidation into request pipeline.(`IMvcBuilder`) 
Two types of Filters are showcased and both work equally well. In each case, the registered `PersonValidator<Person>` is looked up, invoked with `Person` parameter of `PeopleController` method, its FluentValidation rules are executed on the `Person` contract, and if errors are detected, a `BadRequest(400)` with the errors are returned. If validation succeeds, execution is passed on to the controller method.

Two types of validation filters

1. Explicit Validation - `ValidateInputAttribute`
- Selective validation
- Decorate POST and PUT methods with `[ValidateInput]` attribute

2. Implicit (global) validation - `ValidateInputFilter`
- No decoration is required. This filter is an instance of IActionFilter and is hardwired into the request pipeline. It will look at method argument and try to find validator based on its type.


### Person Contract

```csharp
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set;  }
        public DateTime ModifiedOn { get; set; }
    }
```

### Person Validator

```csharp
    public class PersonValidator : AbstractValidator<Person>
    {
        public enum ErrorCode
        {
            FirstNameRequired = 11,
            FirstNameInvalidLength = 12,
            FirstNameHasInvalidChars = 13,

            LastNameRequired = 21,
            LastNameInvalidLength = 22,
            LastNameHasInvalidChars = 23,

            EmailInvalidFormat = 33,
            EmailNotUnique = 34,

            BirthDateInFuture = 41
        }

        public PersonValidator()
        {
            RuleFor(p => p.FirstName)
                  .NotEmpty()
                  .WithName("FirstName")
                  .WithMessage("First name is required.")
                  .WithErrorCode(ErrorCode.FirstNameRequired.ToString())
                  .Length(0, 20)
                  .WithMessage("First name cannot exceed 20 characters.")
                  .WithErrorCode(ErrorCode.FirstNameInvalidLength.ToString())
                  .Matches(@"^[A-Za-z\-\.\s]+$")
                  .WithMessage("First name contains invalid characters.")
                  .WithErrorCode(ErrorCode.FirstNameHasInvalidChars.ToString()); 

            //  remaining  rules not shown
        }
    }

```


## Data Service - Data Access to SQL or Document DB

###  IDataService
This interface provides the gateway into backend data stores.  The responsibility of a dataservice implementation is to provide transformation between contract and data store entities. Although contract and entities are almost identical, they both serve different purposes and help isolate and encapsulate each component layer. Also, contracts often are consolidations of more than one entity, so changing a contract to accommodate this data requirement does not affect the data access tier or have ripple effects throughout the service.

Most importantly, this interface provides loose coupling between the Controller and data access. It allows us to provide all types of the implementations of data access. In this project, we provide two implementations: one is a tradiontional SQL Server Repository using Entity Framework for SQL access. The second data service is an implementation of a DB Document (No SQL) Repository using Microsoft's Document DB SDK, with a Document DB emulator (since not everyone has access to an Azure Document DB instance).

``` csharp
public interface IDataService
{
     List<Person> GetAll();
     Person Get(int id);
     Person Create(Person person);
     Person Update(Person person);
     bool Delete(int id);
}
```
`

### PeopleSqlService

### PeopleDocDbService

## Repository

### SQL Database 

### Azure DocumentDB

## Unit Testing
### Xunit
- NSubstitute
- FluentAssertions

- EF - In-memory DB



## IntegrationTesting
- end to end testing 
- invoke service endpoint with HttpClient and execute right down to DB
- requires setup and teardown strategy
-  
## Test Server
