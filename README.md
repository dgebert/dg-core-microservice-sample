# Microservice Sample in ASP.NET Core

## Introduction

This is a sample demonstration of a simple microservice developed with MIcrosoft's  ASP.NET Core technology stack. It features many components and features of the ASP.NET Core platform.

##  API Deveopment Prerequisites

There is a vast amount of technology resources online. The following is a short list of prerequisites with links to recommended documentation for getting started:

- [.NET Core](https://msdn.microsoft.com/en-us/library/dn878908(v=vs.110).aspx)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/ "ASP.NET Core is a new open-source and cross-platform framework for building modern cloud based internet connected applications, such as web apps, IoT apps and mobile backends")
- [.NET Core on github](https://dotnet.github.io/)
- [EF Core](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro "Core Entity Framework")
- [Swagger](http://swagger.io/)  and [Open API](https://github.com/OAI/OpenAPI-Specification)
- [REST Fundamentals](https://code.tutsplus.com/tutorials/a-beginners-guide-to-http-and-rest--net-16340)
- [Microservices](https://martinfowler.com/articles/microservices.html "Microservices overview by Martin Fowler")

## Getting Started

Here are some great place to start: 
- [Get started with .NET Core](https://docs.microsoft.com/en-us/dotnet/core/get-started)
- [.NET Core on github](https://dotnet.github.io/)
- [Prerequisites for .NET Core on Windows](https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites?tabs=netcore2x)

### 1.  Install IDE - Visual Studio 2017 or Visual Studio Code (you'll need C# plugin for VSCode)

[Visual Studio Downloads](https://www.visualstudio.com/downloads/)

### 2. install [.NET Core 2.1.4 SDK](https://www.microsoft.com/net/download/all)

 If you're adventurous, check out these IDE's:
- [Visual Studio Code](https://code.visualstudio.com/)
- [JetBrains Rider](https://www.jetbrains.com/rider/download/)

### 3. Download solution or clone repo

### 4. Publish `db.sqldatabase` project to a SQL Server instance with 'Contacts' as name of database. 
- This will create database and tables from scripts in project
>Note: You will need to change the db connection string to point to this sql server instance and db in `appsettings.json` in these projects:  `dg.api` and `dg.api.integrationtest`.

### 5. Run all unit and integration tests within the `test` folder.

See README.md in root folder for more details of the microservice architecture and ASP.NET Core features.


