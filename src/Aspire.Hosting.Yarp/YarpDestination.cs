// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace Aspire.Hosting.Yarp;

/// <summary>
/// Represents a destination for YARP routes
/// </summary>
public class YarpDestination(IResourceBuilder<YarpResource> parent, EndpointReference endpoint)
{
    internal IResourceBuilder<YarpResource> ParentBuilder { get; } = parent;

    internal ClusterConfig ClusterConfig { get; private set; } = new()
    {
        ClusterId = $"cluster_{Guid.NewGuid().ToString("N")}",
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "destination1", new DestinationConfig { Address = $"{endpoint.Scheme}://{endpoint.Resource.Name}" } },
        }
    };

    internal void Configure(Func<ClusterConfig, ClusterConfig> configure)
    {
        ClusterConfig = configure(ClusterConfig);
    }
}

/// <summary>
/// Provides extension methods for configuring a YARP destination
/// </summary>
public static class YarpDestinationExtensions
{
    /// <summary>
    /// Set the ForwarderRequestConfig for the destination
    /// </summary>
    public static YarpDestination WithForwarderRequestConfig(this YarpDestination destination, ForwarderRequestConfig config)
    {
        destination.Configure(c => c with { HttpRequest = config });
        return destination;
    }

    /// <summary>
    /// Set the ForwarderRequestConfig for the destination
    /// </summary>
    public static YarpDestination WithForwarderRequestConfig(this YarpDestination destination, HttpClientConfig config)
    {
        destination.Configure(c => c with { HttpClient = config });
        return destination;
    }
}
