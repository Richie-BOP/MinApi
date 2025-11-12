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
        return builder.WithOpenApiDocs(
            "open-api-doc", "OpenAPI document",
            0, "/openapi/v1.json",
            "DocumentChevronDouble", IconVariant.Regular);
    }

    internal static IResourceBuilder<T> WithSwaggerUI<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("swagger-ui-docs", "Swagger UI Docs",
            0, "/swagger",
            "Document", IconVariant.Filled);
    }

    internal static IResourceBuilder<T> WithReDoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs(
            "redocs-docs", "ReDocs UI Docs",
            0, "/api-docs",
            "Document", IconVariant.Filled);
    }

    internal static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs(
            "scalar-docs", "Scalar UI Docs",
            0, "/scalar",
            "Document", IconVariant.Filled);
    }

    private static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder,
        string name,
        string displayName,
        int port,
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
                var endpoint = builder.GetEndpoint("https");

                var url = $"{endpoint.Url}{path}";

                if (port != 0)
                {
                    url = $"{endpoint.Scheme}://{endpoint.Host}:{port}{path}";
                }

                await Task.Run(() =>
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
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
        }
        );
    }
}
