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

Here is a great place to start: [.NET Core on github](https://dotnet.github.io/)

### 1.  Install IDE - Visual Studio 2015 or 2017
>Note:  .NET Core development differs considerably between these two IDE's.  Visual Studio 2017 contains several new and improved features. The following resources best explain all this:
- [What's new in Visual Studio 2017](https://www.visualstudio.com/vs/whatsnew/)
- [Visual Studio 2017](https://www.visualstudio.com/en-us/news/releasenotes/vs2017-relnotes)  Release Notes
- [Visual Studio 2017 RC review: a look at what’s new and improved](https://raygun.com/blog/2016/12/visual-studio-2017-rc-review/)

If you install Visual Studio 2017, all .NET Core components are included

### 2. If you are running Visual Studio 2015 on Windows, install [.NET Core 1.1.1 SDK](https://www.microsoft.com/net/core#windowscmd)

 If you're adventurous, check out these IDE's:
- [Visual Studio Code](https://code.visualstudio.com/)
- [JetBrains Rider](https://www.jetbrains.com/rider/download/)

### 3. Download solution or clone repo

### 4. Publish `db.sqldatabase` project to a SQL Server instance.  You will need to change the connection string
in these projects:  `dg.api` and `dg.api.integrationtest`.

### 5. Run all unit and integration tests within the `test` folder.

See README.md in root folder for more details of the microservice architecture and ASP.NET Core features.

### Document DB Emulator

[Document DB Emulator](https://docs.microsoft.com/en-us/azure/documentdb/documentdb-nosql-local-emulator)
"The Azure DocumentDB Emulator provides a local environment that emulates the Azure DocumentDB service for development purposes. Using the DocumentDB Emulator, you can develop and test your application locally, without creating an Azure subscription or incurring any costs. When you're satisfied with how your application is working in the DocumentDB Emulator, you can switch to using an Azure DocumentDB account in the cloud."

The DocumentDB Emulator provides a high-fidelity emulation of the DocumentDB service. It supports identical functionality as Azure DocumentDB, including support for creating and querying JSON documents, provisioning and scaling collections, and executing stored procedures and triggers. You can develop and test applications using the DocumentDB Emulator, and deploy them to Azure at global scale by just making a single configuration change.

You can use any supported DocumentDB SDK or the DocumentDB REST API to interact with the emulator, as well as existing tools such as the DocumentDB data migration tool and DocumentDB studio.  You can even migrate data between the DocumentDB emulator and the Azure DocumentDB service.

While we created a high-fidelity local emulation of the actual DocumentDB service, the implementation of the DocumentDB emulator is different than that of the service. For example, the DocumentDB Emulator uses standard OS components such as the local file system for persistence, and HTTPS protocol stack for connectivity. This means that some functionality that relies on Azure infrastructure like global replication, single-digit millisecond latency for reads/writes, and tunable consistency levels are not available via the DocumentDB Emulator.