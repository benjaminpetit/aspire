// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Yarp;

namespace Aspire.Hosting;

/// <summary>
/// TODO
/// </summary>
public static class YarpServiceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public static IResourceBuilder<YarpResource> AddYarp(this IDistributedApplicationBuilder builder, string name, string configFilePath, int? port = null)
    {
        var resource = new YarpResource(name);

        return builder.AddResource(resource)
                      .WithHttpEndpoint(port: port, targetPort: YarpContainerImageTags.Port, name: "http")
                      .WithHttpsEndpoint(port: port, targetPort: YarpContainerImageTags.Port+1, name: "https")
                      .WithImage(YarpContainerImageTags.Image)
                      .WithImageRegistry(YarpContainerImageTags.Registry)
                      .WithBindMount(configFilePath, YarpContainerImageTags.ConfigFilePath, isReadOnly: true)
                      .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
                      //.WithEnvironment("YARP_UNSAFE_OLTP_CERT_ACCEPT_ANY_SERVER_CERTIFICATE", "true")
                      .WithOtlpExporter();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    public static IResourceBuilder<YarpResource> UseDevelopmentCertificate(this IResourceBuilder<YarpResource> resource)
    {

        // TODO BPETIT make it magically work
        const string sourceCertificate = @"c:\tmp\local_dev.cer";
        const string destinatinonCertificate = @"/etc/pki/tls/certs/ce275665.0";

        return resource.WithBindMount(sourceCertificate, destinatinonCertificate);
    }
}
