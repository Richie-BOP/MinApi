using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddCarter();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        if (context.ProblemDetails is HttpValidationProblemDetails validationProblemDetails)
        {
            context.ProblemDetails.Detail = $"Error(s) occrred: {validationProblemDetails.Errors.Values.Sum(x => x.Length)}";
            
            var namingPolicy = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>()
                .Value.JsonSerializerOptions.PropertyNamingPolicy;
            
            if (namingPolicy is not null)
            {
                validationProblemDetails.Errors = validationProblemDetails.Errors
                    .ToDictionary(
                        kvp => namingPolicy.ConvertName(kvp.Key),
                        kvp => kvp.Value
                    );
            }
        }
    };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapCarter();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

await app.RunAsync();
