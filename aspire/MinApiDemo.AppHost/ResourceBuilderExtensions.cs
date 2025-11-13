using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace MinApiDemo.AppHost;

// https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/custom-resource-commands
// https://www.fluentui-blazor.net/Icon#explorer
internal static class ResourceBuilderExtensions
{
    internal static IResourceBuilder<T> WithOpenApi<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiCommand(
            "open-api-doc",
            "OpenAPI document",
            "openapi/v1.json",
            "DocumentChevronDouble",
            IconVariant.Regular);
    }

    internal static IResourceBuilder<T> WithSwaggerUI<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiCommand(
            "swagger-ui-docs", 
            "Swagger UI Docs",
            "swagger",
            "Document", 
            IconVariant.Filled);
    }

    internal static IResourceBuilder<T> WithReDoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiCommand(
            "redocs-docs", 
            "ReDocs UI Docs",
            "api-docs",
            "Document", 
            IconVariant.Filled);
    }

    internal static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiCommand(
            "scalar-docs", 
            "Scalar UI Docs",
            "scalar",
            "Document", 
            IconVariant.Filled);
    }

    private static IResourceBuilder<T> WithOpenApiCommand<T>(this IResourceBuilder<T> builder,
        string name,
        string displayName,
        string path,
        string iconName,
        IconVariant iconVariant
        )
        where T : IResourceWithEndpoints
    {
        return builder.WithCommand(
        name,
        displayName,
        executeCommand: async _ =>
        {
            try
            {
                var url = GetResourceUrl(builder, path);

                await Task.Run(() =>
                {
                    Process.Start(new ProcessStartInfo(url.AbsoluteUri) { UseShellExecute = true });
                });

                return new ExecuteCommandResult { Success = true };
            }
            catch (Exception doh)
            {
                return new ExecuteCommandResult { Success = false, ErrorMessage = doh.Message };
            }
        },
        commandOptions: new CommandOptions
        {
            UpdateState = (context) =>
            {
                return context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy ?
                ResourceCommandState.Enabled :
                ResourceCommandState.Disabled;

            },
            IconName = iconName,
            IconVariant = iconVariant
        });
    }

    private static Uri GetResourceUrl<T>(IResourceBuilder<T> builder, string path)
        where T : IResourceWithEndpoints
    {
        builder.Resource.TryGetUrls(out IEnumerable<ResourceUrlAnnotation>? urls);

        var baseUrl = urls?.FirstOrDefault()?.Url ?? builder.Resource.GetEndpoint("https").Url;

        return new Uri(new Uri(baseUrl), path);
    }
}
