# Serilog.Sinks.AspNetCore.SignalR

![Github Actions](https://github.com/lTimeless/Serilog.Sinks.AspNetCore.SignalR/workflows/Github%20Actions/badge.svg)
![Nuget](https://img.shields.io/nuget/v/Serilog.Sinks.AspNetCore.SignalR)

A Serilog sink that writes logs as a string or an object to the given SignalR Hub.

I was Inspired by [serilog-sinks-signalr-core](https://github.com/DrugoLebowski/serilog-sinks-signalr-core) and [serilog-sinks-signalr](https://github.com/serilog/serilog-sinks-signalr), I decided to write my own because non of both worked for me. I always got a Stackoverflow in an Asp.net core 3 application.

## Support
If you encounter any problems or have any suggestions, please help me fix/implement the solution by creating an [issue](https://github.com/lTimeless/Serilog.Sinks.AspNetCore.SignalR/issues)

## Usage and configuration
> __[Update 22.10]__ This also works with .net 5. Th eimplementation is the same!

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
{...}
var hub = ServiceProvider.GetService<IHubContext<MyOwnExampleHub, IExampleHub>>(); // or IHub

Log.Logger = new LoggerConfiguration()
    .WriteTo.SignalRSink<MyOwnExampleHub, IExampleHub>(
        LogEventLevel.Information,
        service,
        new MyCustomProvider(), // can be null
        new string[] {},        // can be null
        new string[] {},        // can be null
        new string[] {},        // can be null
        false);                 // false is the default value
    )
    .CreateLogger();
{...}

```

Asp.Net core (3)
```csharp
// In Programm.cs -> CreateHostBuilder method
{...}
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
                    new string[] {},        // can be null
                    new string[] {},        // can be null
                    new string[] {},        // can be null
                    false);                 // false is the default value
        });
{...}
        
// In Startup.cs -> Configure method   
{...}   
app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");
    
        endpoints.MapHub<LogHub>("<yourPath>");
    });
{...}

```
#### Send methods
With `SendLogAsString` you can send a message as a string formatted like:
```charp
$"{logEvent.Timestamp:dd.mm.yyyy HH:mm:ss.fff} {logEvent.Level.ToString()} {logEvent.RenderMessage(_formatProvider)} {logEvent.Exception?.ToString() ?? "-"}"
//example:  25.07.2020 20:07:23:111 Information This is my test message you write into any logger -
```
With `SendLogAsObject` you can send a message as an object formatted like:
```
new { id, timestamp, level, message, exception}
```
The object has the same logevent properties and also an id prop so frontends can iterate over it while having a unique key for every entry.


### Receive the log events

In the client code, subscribe to the `SendLogAsString` or `SendLogAsObject` method.

C#
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("<yourPath>")
    .Build()

connection.On<string>("SendLogAsString", (string message) => {
    Console.WriteLine(message);
});
```

Javascript / Typescript
```js 
this.connection = new HubConnectionBuilder()
      .withUrl('<yourPath>')
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
