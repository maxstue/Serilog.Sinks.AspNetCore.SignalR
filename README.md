# Serilog.Sinks.AspNetCore.SignalR

![Github Actions](https://github.com/lTimeless/Serilog.Sinks.AspNetCore.SignalR/workflows/Serilog.Sink.Signalr%20Nuget%20Package/badge.svg)
![Nuget](https://img.shields.io/nuget/v/Serilog.Sinks.AspNetCore.SignalR)

A Serilog sink that writes events as string or object to the given SignalR Hub.

I was Inspired by [serilog-sinks-signalr-core](https://github.com/DrugoLebowski/serilog-sinks-signalr-core) and [serilog-sinks-signalr](https://github.com/serilog/serilog-sinks-signalr), I decided to write my own because non of both worked for me. I always got a Stackoverflow in an Asp.net core 3 application.

## Usage and configuration
The hub you want to use with the Sink must inherit from Hub<IHub> or an interface which inherits from it. 
To have the services in the "UseSerilog(....)" method you __need__ to install the package "Serilog.Extensions.Hosting"

An example as follows:

```csharp
public class MyOwnExampleHub : Hub<IExampleHub> { // or IHub
}
```

```csharp
public interface IExampleHub : IHub {
}
```

An example setup is as follows:

.Net core
```csharp 
var hub = ServiceProvider.GetService<IHubContext<MyOwnExampleHub, IExampleHub>>(); // or IHub

Log.Logger = new LoggerConfiguration()
    .WriteTo.SignalRSink(
        hub,
        formatProvider: new CustomFormatProvider(),
        groups: new [] { "group1", },
        userIds: new [] { "user2", },
        excludedConnectionIds: new [] { "1", }
    )
    .CreateLogger();
[...]

```

Asp.Net core (3)
```csharp
// In Programm.cs -> CreateHostBuilder method
private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.UseSerilog((hostingContext, service, loggerConfig) =>
				{
					loggerConfig
						.ReadFrom.Configuration(hostingContext.Configuration)
						.Enrich.FromLogContext()
						.Enrich.WithProcessId()
						.WriteTo.SignalRSink<MyOwnExampleHub, IExampleHub>(
              LogEventLevel.Information,
							service,
							new MyCustomProvider(), // can be null
							new string[] {}, // can be null
							new string[] {}, // can be null
							new string[] {}, // can be null
							false); // false is the default value
				});
        
// In Startup.cs -> Configure method      
app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action=Index}/{id?}");

				endpoints.MapHub<EventHub>("/api/hub");
				endpoints.MapHub<LogHub>("/api/hub/logs");
			});

```


### Receive the log events

In the client code, subscribe to the `SendLogAsString` or `SendLogAsObject` method.

C#
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("<yourURL>")
    .Build()

connection.On<string>("SendLogAsString", (string message) => {
    Console.WriteLine(message);
});
```

Javascript / Typescript
```js 
this.connection = new HubConnectionBuilder()
      .withUrl('<yourURL>')
      .configureLogging(LogLevel.Information)
      .build();
      
this.connection
      .start()
      .then(() => {
        this.connection.on('SendLogAsObject', (data: any) => {
          console.log(data);
        });
      })
      .catch((err: any) => console.error(err));
```
