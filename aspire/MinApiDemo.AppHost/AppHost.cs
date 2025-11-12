using MinApiDemo.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddProject<Projects.MinApiDemo_Api>("minapidemo-api")
    .WithOpenApi()
    .WithSwaggerUI()
    .WithReDoc()
    .WithScalar();

await builder
    .Build().RunAsync();
