# Microservice Sample in ASP.NET Core

## Introduction

This is a sample demonstration of a simple microservice developed with MIcrosoft's  ASP.NET Core technology stack. It features many components and features of the ASP.NET Core platform.


##  API Deveopment Prerequisites

There is a vast amount of technology resources online. The following is a short list of prerequisites with links to recommended documentation for getting started:

- [.NET Core](https://msdn.microsoft.com/en-us/library/dn878908(v=vs.110).aspx)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/ "ASP.NET Core is a new open-source and cross-platform framework for building modern cloud based internet connected applications, such as web apps, IoT apps and mobile backends")
- [.NET Core on github](https://dotnet.github.io/)
- [EF Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro "Core Entity Framework")
- [Swagger](http://swagger.io/)
- [Open API](https://github.com/OAI/OpenAPI-Specification)
- [REST Fundamentals](https://code.tutsplus.com/tutorials/a-beginners-guide-to-http-and-rest--net-16340)
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

![](./CoreWebApiArchitecture.png "Microsservice Web API Architecture")

Example: People service

### API Operations

| Operation | Route | Description | Request Body | Response Body | Status Codes
| --- | --- | --- | --- | --- | ---
| GET | /people | Get all people | None | List of Person contracts | 200, 404, 500
| GET | /people/{id} | Get a person by id | None | Person contract | 200, 404, 500
| POST | /people | Add a person  | Person contract | Person contract | 201, 400, 500
| PUT | /people | Update an existing  person | Person Contract | None | 201, 400, 404, 500
| DELETE | /people/{id} | Delete a person by Id | None | None | 200, 404 ,500


### Projects/Assemblies

| Project | Description | 
| --- | --- | ---
| `dg.api` | Web API project - Controller providing endpoints for Person CRUD operations| 
| `dg.common.exceptionhandling`  | Exception handling framework components |
| `dg.common.logging`  | Logging components |
| `dg.common.validation`  | Input (api operation contract) validation components  |
| `dg.contract` | Contracts for web api operations   |       
| `dg.dataservice` | Data access with EF Core    |       
| `dg.document.db` | Data access with Document DB sdk  |       
| `dg.repository` | EF DB context with POCO models - generated from SQL database   |                                     
| `dg.sqldatabase` | SQL database containing People DB schema and seed scripts|  
| `dg.validator` | FluentValidation validator for Person contract |  
| `dg.api.test` | Integration testing for PersonController, using ASP.NET Core in-memory TestHost to host Web API |  
| `dg.dataservice.test` | Unit testing for SQL data service using in-memory DB  provided by EF Core |  
| `dg.validator test.test` | Unit testing for Controllers, Validators etc |  




### Dependency Injection

### Logging

### Exception Handling

### Validation
- FluentValidation
- Extensions 
	- Action Attribute Filter (per operation)
	- Action Filter (global)

### Data Access

### PeopleController

## Contract

## Data Service

## Repository

## SQL Database 

## Azure DocumentDB

## Unit Testing
### Xunit
### NSubstitute
### FluentAssertions

### PersonValidator
### PeopleService

## IntegrationTesting
### Test Server
### REST Sharp