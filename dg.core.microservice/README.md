# Microservice Sample in ASP.NET Core - Example:  People Service

## Microservice Architecture

![](../CoreWebApiArchitecture.PNG?raw=true "Microsservice Web API Architecture") 

### People Service 
The sample microservice implementation is a people service which provides CRUD access to a backend People db.  A data service encapsulates the different db data access implementations. Currently there are two DB implementations: SQL Server and Document DB (no SQL). Other target stores - queues, files, message bus, etc - could also be implemented and abstracted with this data service layer.


### API Operations

| Operation | Route | Description | Parameter or Request Body  | Response Body | Status Codes
| --- | --- | --- | --- | --- | ---
| GET | /people | Get all people | None | List of Person contracts | 200, 404, 500
| GET | /people/{id} | Get a person by id | id of Person | Person contract | 200, 404, 500
| POST | /people | Add a person  | Person contract | Person contract | 201, 400, 409, 500
| PUT | /people | Update an existing  person | Person Contract | None | 201, 400, 404, 500
| DELETE | /people/{id} | Delete a person by Id | id of Person | None | 200, 404 ,500

## Swagger generated docs

![](../Swagger.PNG?raw=true "Swagger provides a way to describe API - JSON outlines the names, order, and other details of API.") 

### Projects/Assemblies

| Project | Description | 
| --- | --- | 
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

###  IPeopleService
This interface provides the gateway into backend data stores.  The responsibility of a dataservice implementation is to provide transformation between contract and data store entities. Although contract and entities are almost identical, they both serve different purposes and help isolate and encapsulate each component layer. Also, contracts often are consolidations of more than one entity, so changing a contract to accommodate this data requirement does not affect the data access tier or have ripple effects throughout the service.

Most importantly, this interface provides loose coupling between the Controller and data access. It allows us to provide all types of the implementations of data access. In this project, we provide two implementations: `PeopleSqlService` is a tradiontional SQL Server Repository using Entity Framework for SQL access. `PeopleDocDbService` is an implementation of a DB Document (No SQL) Repository using Microsoft's Document DB SDK, with a Document DB emulator (since not everyone has access to an Azure Document DB instance).


`IPeopleService.cs`

``` csharp
public interface IPeopleService
{
    List<Person> GetAll();
    Person Get(int id);
    Person Create(Person p);
    Person Update(Person p);
    bool Delete(int id);
    Person Find(Person p);
}
```

For implementations, see `PeopleSqlService` and `PeopleDocDbService`.


## Unit Testing 

### Xunit
Unit tests use the latest `Xunit` framework. Some of the advantages of this framework over NUnit, MSUnit, etc, is its growing popularity, adoption by Microsoft, and its .NET Core support.  In fact, Visual Studio 2017 includes built-in support for xUnit.net projects, as does `dotnet new`.

If you are new to Xunit, here are some useful resources to start with:

- [Xunit home](https://xunit.github.io/)
- [Comparing Xunit to other frameworks](https://xunit.github.io/docs/comparisons.html)

An important feature of XUnit is its  test lifecycle support. One time test fixture setup is separated in a `ClassFixture<T>` template. Test setup/teardown are supported by C# language constructs - constructor and `Dispose()`, which is most efficient facility for tear down, since this will always get called.

### NSubstitute
[NSubstittue](http://nsubstitute.github.io/) might be the simplest, most succinct, and easy to use mocking library. The easiest way to understand it, is by using it and exploring its features. It has also kept up with the volatile evoluation of .NET Core. 

### FluentAssertions
All mocking libraries provide `Assert` functionality. [Fluent Assertions](http://fluentassertions.com/) goes farther with its comprehensive set of extension methods that allow you to more naturally specify expected outcomes with its fluent syntax.

### .NET Core features supporting unit testing
Unit tests employ various ASP.NET Core features to isolate the SUT (system or subject under test). A summary of main features is as follows: 

### 1. TestServer
This implements in-mempry hosting of the API and supplies an HttpClient to execute URI endpoints for POST and PUT operations in order to test validation action attribute and action filters in isolation. It supports complete configuration and dependcncy injection of mock services to isolate functionality of action filters.

### 2. EF Core In-memory DB 
This feature allows testing of `PeopleSqlService`'s  EF Linq to SQL implementation without mocking its `PeopleContext`.  Here is a quick look at the `PeopleSqlServiceTest` demonstrating in-memory configuration for  `PeopleContext` without mocking. 

`PeopleSqlServiceTest.cs`
```csharp

 public class PeopleSqlServiceTest
{
    DbContextOptions<PeopleContext> _options;
    public PeopleSqlServiceTest()
    {
        // Create a fresh service provider, and therefore a fresh 
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        _options = new DbContextOptionsBuilder<PeopleContext>()
                            .UseInMemoryDatabase(databaseName: "People")
                            .UseInternalServiceProvider(serviceProvider)
                            .Options;
    }

    [Fact]
    public void GivenPersonExists_WhenGetById_ShouldReturnPerson()
    {
        Person p = null;
        int id = 1;
        using (var db = new PeopleContext(_options))
        {
            p = BuildPerson(id);
            db.Person.Add(p);
            db.SaveChanges();
        }

        using (var db = new PeopleContext(_options))
        {
            var service = new PeopleSqlService(db);

            var person = service.Get(id);
            person.Should().NotBeNull();
            person.ShouldBeEquivalentTo(p);
        }
    }

 // ...

}
```

### 3. Unit Tests
The following compoenents are covered by unit testing:

- `ValidateInputAttribute` 
- `ValidateInputFilter`
- `PeopleController`
- `PeopleSqlService` 
- `PersonValidator`

## Integration Testing
This also uses TestServer to execute API Controller logic via URI endpoints. The Web API, as is, is configured with its real services. A custom `TestServerFxiture` supplies this web hosting. It also implements Setup and Teardown of the database after each test run.

Test Fixture:

`TestServerFixture.cs`

```csharp

public class TestServerFixture: IDisposable
{
    public const string ConnStringKey = "ConnectionStrings:DefaultConnection";
    private string ConnectionString;

    public HttpClient Client { get; }
    public TestServer Server { get; }
    public IConfigurationRoot Configuration { get; }
    public DbContextOptions<PeopleContext> DbContextOptions { get;  }

    public TestServerFixture()
    {
        var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
        Configuration = builder.Build();
        ConnectionString = Configuration[ConnStringKey];
 
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new ArgumentException("Cannot find connection string - check appsettings or where it is (not) located");
        }

        var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(Configuration)
                .UseKestrel()
                // Configure
                .Configure(app => app.UseMvc())
                .ConfigureServices(ConfigureServices);
        ;
        Server = new TestServer(webHostBuilder);
        Client = Server.CreateClient();
        Client.BaseAddress = new Uri(@"http://localhost:5000/");
    }

    // Dependency Injection - register mvc, services, db
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddValidateInputAttribute<PersonValidator>();
        services.AddDbContext<PeopleContext>(options => options.UseSqlServer(ConnectionString));     
        services.AddScoped<IPeopleService, PeopleSqlService>();
    }

    // ...
}
```

Test sample: 

`PeopleApiTest.cs`

```csharp
 public class PeopleApiTest : IClassFixture<TestServerFixture>, IDisposable
{
    private TestServerFixture _fixture;

    public PeopleApiTest(TestServerFixture fixture)
    {
        _fixture = fixture;
        _fixture.SetUpDb();
    }

    public void Dispose()
    {
        _fixture.TearDownDb();
    }


    [Fact]
    public async Task GivenNoPersonExists_WhenGet_ShouldReturnNotFound)
    {
        var response = await _fixture.Client.GetAsync("people/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenPersonExists_WhenGet_ShouldReturnNotFound()
    {
        using (var db = _fixture.GetDb())
        {
            try
            {
                var p = new PeopleBuilder().Build();
                var personEntity = p.ToPersonEntity();
                db.Person.Add(personEntity);
                db.SaveChanges();

                var uri = string.Format("people/{0}", personEntity.Id);
                var response = await _fixture.Client.GetAsync(uri);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var personInDb = response.GetResult<Person>();
                personInDb.ShouldBeEquivalentTo(p);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
                // TODO: Handle failure
            }
        }
    }

    // ...
}

```
