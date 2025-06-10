// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Yarp;
using Yarp.ReverseProxy.Configuration;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding YARP resources to the application model.
/// </summary>
public static class YarpServiceExtensions
{
    private const int Port = 5000;

    private const string ConfigDirectory = "/etc";

    private const string ConfigFileName = "yarp.config";

    /// <summary>
    /// Adds a YARP container to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<YarpResource> AddYarp(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name)
    {
        var resource = new YarpResource(name);

        var yarpBuilder = builder.AddResource(resource)
                      .WithHttpEndpoint(targetPort: Port)
                      .WithImage(YarpContainerImageTags.Image)
                      .WithImageRegistry(YarpContainerImageTags.Registry)
                      .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
                      .WithOtlpExporter();

        if (builder.ExecutionContext.IsRunMode)
        {
            // YARP will not trust the cert used by Aspire otlp endpoint when running locally
            // The Aspire otlp endpoint uses the dev cert, only valid for localhost, but from the container
            // perspective, the url will be something like https://docker.host.internal, so it will NOT be valid.
            yarpBuilder.WithEnvironment("YARP_UNSAFE_OLTP_CERT_ACCEPT_ANY_SERVER_CERTIFICATE", "true");
        }

        // Map the configuration file
        yarpBuilder.WithContainerFiles(ConfigDirectory, async (context, ct) =>
        {
            var contents = await yarpBuilder.Resource.ConfigurationBuilder.Build(ct).ConfigureAwait(false);

            var configFile = new ContainerFile
            {
                Name = ConfigFileName,
                Contents = contents
            };

            return [configFile];
        });

        return yarpBuilder;
    }

    /// <summary>
    /// Set explicitly the config file to use for YARP.
    /// </summary>
    /// <param name="builder">The YARP resource to configure.</param>
    /// <param name="configFilePath">The path to the YARP config file.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<YarpResource> WithConfigFile(this IResourceBuilder<YarpResource> builder, string configFilePath)
    {
        builder.Resource.ConfigurationBuilder.WithConfigFile(configFilePath);
        return builder;
    }

    /// <summary>
    /// Configure the YARP resource.
    /// </summary>
    /// <param name="builder">The YARP resource to configure.</param>
    /// <param name="configurationBuilder">The delegate to configure YARP.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<YarpResource> Configure(this IResourceBuilder<YarpResource> builder, Action<IYarpConfigurationBuilder> configurationBuilder)
    {
        configurationBuilder(builder.Resource.ConfigurationBuilder);
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="proxiedResource"></param>
    /// <param name="routeMatch"></param>
    /// <param name="order"></param>
    /// <param name="authorizationPolicy"></param>
    /// <param name="rateLimiterPolicy"></param>
    /// <param name="outputCachePolicy"></param>
    /// <param name="timeoutPolicy"></param>
    /// <param name="timeout"></param>
    /// <param name="corsPolicy"></param>
    /// <param name="maxRequestBodySize"></param>
    /// <param name="metadata"></param>
    /// <param name="transforms"></param>
    /// <returns></returns>
    public static IResourceBuilder<YarpResource> WithReference(
        this IResourceBuilder<YarpResource> builder,
        EndpointReference proxiedResource,
        RouteMatch routeMatch,
        int? order = null,
        string? authorizationPolicy = null,
        string? rateLimiterPolicy = null,
        string? outputCachePolicy = null,
        string? timeoutPolicy = null,
        TimeSpan? timeout = null,
        string? corsPolicy = null,
        long? maxRequestBodySize = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        IReadOnlyList<IReadOnlyDictionary<string, string>>? transforms = null)
    {
        var id = Guid.NewGuid().ToString("N");
        var clusterId = $"cluster_{proxiedResource.Resource.Name}_{id}";
        var routeConfig = new RouteConfig
        {
            RouteId = $"route_{proxiedResource.Resource.Name}_{id}",
            ClusterId = clusterId,
            Match = routeMatch,
            Order = order,
            AuthorizationPolicy = authorizationPolicy,
            RateLimiterPolicy = rateLimiterPolicy,
            OutputCachePolicy = outputCachePolicy,
            TimeoutPolicy = timeoutPolicy,
            Timeout = timeout,
            CorsPolicy = corsPolicy,
            MaxRequestBodySize = maxRequestBodySize,
            Metadata = metadata,
            Transforms = transforms,
        };
        var clusterConfig = new ClusterConfig
        {
            ClusterId = clusterId,
            Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
            {
                { "destination0", new DestinationConfig { Address = $"{proxiedResource.Scheme}://{proxiedResource.Resource.Name}"} }
            },
        };
        builder.Configure(configuration =>
        {
            configuration
                .AddRoute(routeConfig)
                .AddCluster(clusterConfig);
        });
        return builder.WithReference(proxiedResource);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="proxiedResource"></param>
    /// <param name="routeMatch"></param>
    /// <param name="transforms"></param>
    /// <param name="order"></param>
    /// <param name="authorizationPolicy"></param>
    /// <param name="rateLimiterPolicy"></param>
    /// <param name="outputCachePolicy"></param>
    /// <param name="timeoutPolicy"></param>
    /// <param name="timeout"></param>
    /// <param name="corsPolicy"></param>
    /// <param name="maxRequestBodySize"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public static IResourceBuilder<YarpResource> WithReference(
        this IResourceBuilder<YarpResource> builder,
        EndpointReference proxiedResource,
        RouteMatch routeMatch,
        Action<RouteTransformsBuilder> transforms,
        int? order = null,
        string? authorizationPolicy = null,
        string? rateLimiterPolicy = null,
        string? outputCachePolicy = null,
        string? timeoutPolicy = null,
        TimeSpan? timeout = null,
        string? corsPolicy = null,
        long? maxRequestBodySize = null,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        var routeTransforms = new RouteTransformsBuilder();
        transforms(routeTransforms);
        return builder.WithReference(
            proxiedResource,
            routeMatch,
            order,
            authorizationPolicy,
            rateLimiterPolicy,
            outputCachePolicy,
            timeoutPolicy,
            timeout,
            corsPolicy,
            maxRequestBodySize,
            metadata,
            routeTransforms.Build());
    }
}
