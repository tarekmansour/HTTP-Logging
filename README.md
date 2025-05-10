# HTTP-Logging
.NET HTTP Logging with [Seq](https://datalust.co/seq).

## Seq
I used `Seq` to write logs to a structured log server. Seq is a powerful log server that allows you to store, search, and analyze logs in a structured format. It provides a web interface for querying and visualizing logs, making it easy to find and troubleshoot issues in your application.

### Configuration
- Get `Seq` [docker image](https://hub.docker.com/r/datalust/seq).
- Add the NuGet package to your project:
```csharp
dotnet add package Seq.Extensions.Logging
```
- Call `AddSeq()` on the loggingBuilder provided by `AddLogging()`:
```csharp
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq();
});
```

## HTTP Logging
This library provides a middleware for logging HTTP requests and responses in .NET applications. It captures detailed information about the HTTP traffic, including headers, query parameters, and body content, and logs it in a structured format.

ASP.NET Core 8 introduced a set of additional configuration options that I believe are essential to make the library more robust and flexible. These options include:
- **LoggingFields**: A collection of fields that you want to log. This allows you to customize the logging output based on your specific needs.
- **CombineLogs**: A boolean flag that determines whether to combine the request and response logs into a single log entry. This is useful for simplifying log analysis and reducing the number of log entries.
- **LogLevel**: The minimum log level required to log the request and response. This allows you to filter out less important logs and focus on the most relevant information.
- etc.

### Configuration
- checkout the official documentation: [HTTP logging in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-8.0)

## Interceptors
The configuration options for HTTP logging are somewhat limited, primarily allowing you to specify which fields and headers should be logged. This is where interceptors become particularly useful.

By default, HTTP logging captures and logs all requests and responses. However, if you need to exclude specific requests or responses from being logged—such as health check endpoints, static files, or sensitive data—you can implement the `IHttpLoggingInterceptor` interface.

The `IHttpLoggingInterceptor` interface allows you to define custom logic to filter out requests or responses based on your application's requirements. Once implemented, the interceptor can be registered in your application's dependency injection container, enabling fine-grained control over what gets logged.