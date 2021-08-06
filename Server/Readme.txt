This readme explains the structure of the solution. 

1. AppModels project
This project stores the models and ESqls converted and exported from the PowerBuilder application(s).
|-- AppModels
	|-- PowerBuilder Application #1
		|-- ESqls									These are converted from the embedded SQLs in the application.
		|-- Models									These are converted from the DataWindows in the application.
			|-- PBL #1
			|-- PBL #2
	|-- PowerBuilder Application #2
	...


2. ServerAPIs project
This is the PowerServer Web API project. It contains all the necessary configurations, Web API controllers and services, and the supporting modules (authentication, logging, healthcheck, and so on).
|-- ServerAPIs
	|-- Properties
		|-- launchSettings.json						Application profile settings used in the local development machine

	|-- AppConfig									Configuration management module
		|-- AppConfig.json							Stores the startup configuration of the current module
		|-- AppConfig.xml							Stores the project management settings of the current module, such as NuGet package reference, MSBuild settings
		|-- AppConfigExtensions.cs					Registers services at the startup of the module, such as injecting services to the container, and injecting handlers to the request pipeline
		|-- Applications.json						Stores the application configuration and database connection configuration (the DB connection profile being "Default")
		|-- Applications.Development.json			Stores the application configuration and database connection configuration (the DB connection profile being "Development")
		|-- Applications.<connection profile>.json	Stores the application configuration and database connection configuration (the DB connection profile other than Default and Development)

	|-- Authentication								Authentication module
		|-- Authentication.json						Stores the startup configuration of the current module
		|-- Authentication.xml						Stores the project management settings of the current module, such as NuGet package reference, MSBuild settings
		|-- AuthenticationExtensions.cs				Registers services at the startup of the module, such as injecting services to the container, and injecting handlers to the request pipeline

	|-- Controllers									PowerServer controller APIs
		|-- ApplicationController.cs				Application configuration management APIs
		|-- ConnectionController.cs					Connection configuration management APIs
		|-- LicenseController.cs					License information query APIs
		|-- SessionController.cs					Session management APIs
		|-- StatisticsController.cs					Statistics information query APIs
		|-- TransactionController.cs				Transaction management APIs

	|-- HealthChecks								PowerServer health checking module
		|-- health-ui								Output UI for the Health checking results
		|-- HealthChecks.json						Stores the startup configuration of the current module
		|-- HealthChecks.xml						Stores the project management settings of the current module, such as NuGet package reference, MSBuild settings
		|-- HealthChecksExtensions.cs				Registers services at the startup of the module, such as injecting services to the container, and injecting handlers to the request pipeline
		|-- HealthChecksPublisher.cs				Provides the publisher of the health check report
		|-- HealthChecksUIResponseWriter.cs			Provides the writer to write the health check result into the UI

	|-- Logging										PowerServer logging module
		|-- Logging.json							Stores the startup configuration of the current module in the default environment
		|-- Logging.Development.json				Stores the startup configuration of the current module in the Development environment
		|-- Logging.xml								Stores the project management settings of the current module, such as NuGet package reference, MSBuild settings
		|-- LoggingExtensions.cs					Registers services at the startup of the module, such as injecting services to the container, and injecting handlers to the request pipeline
		|-- Log4Net.xml								Stores the settings on how to output the logs

	|-- OpenAPI										OpenAPI module
		|-- OpenAPI.json							Stores the startup configuration of the current module
		|-- OpenAPI.xml								Stores the project management settings of the current module, such as NuGet package reference, MSBuild settings
		|-- OpenAPIExtensions.cs					Registers services at the startup of the module, such as injecting services to the container, and injecting handlers to the request pipeline
		|-- Swagger.xml								The output file of the OpenAPI comments
	|-- Program.cs									PowerServer program file, for configuring the PowerServer server
	|-- Startup.cs									PowerServer startup file, to inject services to the container, and inject handlers to the request pipeline
	|-- Server.json									PowerServer server configuration file in the default environment
	|-- Server.Development.json						PowerServer server configuration file in the Development environment


3. ServerAPIs.Test project
Using this project to test whether PowerServer Web APIs are accessible
|-- ServerAPIs.Tests
	|-- ServerAPIs.Host.cs							Starts the test server for the subsequent tests
	|-- ServerAPIs.Test.cs							Stores the common test functions for testing the PowerServer APIs