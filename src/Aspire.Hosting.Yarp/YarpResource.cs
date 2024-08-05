// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting.Yarp;

/// <summary>
/// Represents a YARP resource.
/// </summary>
public class YarpResource : ContainerResource, IResourceWithConnectionString
{
    private readonly EndpointReference _endpoint;

    /// <summary>
    /// Initialize a new <see cref="YarpResource"/> instance.
    /// </summary>
    /// <param name="name">The name of the YARP instance.</param>
    public YarpResource(string name) : base(name)
    {
        _endpoint = new EndpointReference(this, "tcp");
    }

    /// <summary>
    /// Path of the config file to use
    /// </summary>
    public string? ConfigFilePath { get; set; }

    /// <summary>
    /// Gets the connection string expression for the Yarp instance.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression
    {
        get
        {
            var value = ReferenceExpression.Create(
                $"http://{_endpoint.Property(EndpointProperty.Host)}:{_endpoint.Property(EndpointProperty.Port)}");
            return value;
        }
    }
}
