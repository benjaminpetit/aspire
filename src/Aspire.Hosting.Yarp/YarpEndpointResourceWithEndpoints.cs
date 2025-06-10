// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Yarp;
using Yarp.ReverseProxy.Configuration;

namespace Aspire.Hosting;

/// <summary>
/// 
/// </summary>
public static class YarpEndpointResourceWithEndpoints
{
    /// <summary>
    /// 
    /// </summary>
    public static IResourceBuilder<T> WithYarp<T>(this IResourceBuilder<T> builder, Action<IYarpEndpointConfigurator> configurator) where T : IResourceWithEndpoints
    {
        var yarp = builder.ApplicationBuilder.Resources.OfType<YarpResource>().SingleOrDefault();
        if (yarp is null)
        {
            yarp = builder.ApplicationBuilder.AddYarp("default").Resource!;
        }
        var endpointConfigurator = new YarpEndpointConfigurator(yarp, builder.Resource.GetEndpoint("http"));
        configurator(endpointConfigurator);
        endpointConfigurator.AddCluster();
        return builder;
    }
}

/// <summary>
/// 
/// </summary>
public interface IYarpEndpointConfigurator
{
    /// <summary>
    /// 
    /// </summary>
    public IYarpEndpointConfigurator AddRoute(
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
        IReadOnlyList<IReadOnlyDictionary<string, string>>? transforms = null);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IYarpEndpointConfigurator ConfigureCluster();
}

internal sealed class YarpEndpointConfigurator : IYarpEndpointConfigurator
{
    private int _routeCounter;
    private readonly string _routeId;
    private readonly string _clusterId;
    private readonly YarpResource _yarp;
    private readonly ClusterConfig _clusterConfig;

    public YarpEndpointConfigurator(YarpResource yarp, EndpointReference endpointReference)
    {
        _routeId = $"{endpointReference.EndpointName}_route";
        _clusterId = $"{endpointReference.EndpointName}_cluster";
        _clusterConfig = new ClusterConfig { ClusterId = _clusterId };
        _yarp = yarp;
    }

    public IYarpEndpointConfigurator AddRoute(
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
        var routeConfig = new RouteConfig
        {
            RouteId = $"{_routeId}.{_routeCounter++}",
            ClusterId = _clusterId,
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
        _yarp.ConfigurationBuilder.AddRoute(routeConfig);

        return this;
    }

    public IYarpEndpointConfigurator ConfigureCluster()
    {
        throw new NotImplementedException();
    }

    internal IYarpEndpointConfigurator AddCluster()
    {
        _yarp.ConfigurationBuilder.AddCluster(_clusterConfig);
        return this;
    }
}
