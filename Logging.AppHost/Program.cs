var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Logging_API>("logging-api");

builder.Build().Run();
