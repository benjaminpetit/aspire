// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Yarp;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding YARP resources to the application model.
/// </summary>
public static class YarpBuilderExtensions
{

    private const string DockerFileContent = @"
FROM {0}
COPY {1} ""/etc/yarp/yarp.config""
    ";

    /// <summary>
    /// Add a YARP container to the application model
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="config">The path to the config file to use.</param>
    /// <param name="port">The host port to bind the underlying container to.</param>
    /// <returns></returns>
    public static IResourceBuilder<YarpResource> AddYarp(this IDistributedApplicationBuilder builder, string name, string config, int port)
    {
        var configFile = new FileInfo(config);
        if (!configFile.Exists)
        {
            // TODO BPETIT
        }

        var yarp = new YarpResource(name);
        return builder
            .AddResource(yarp)
            .WithImage(YarpContainerImageTags.Image, YarpContainerImageTags.Tag) // Not sure why it's needed
            .WithDockerfile(CreateTempDockerfile(configFile))
            //.WithBindMount(config, "/etc/yarp/yarp.config")
            .WithHttpsEndpoint(port, 5001, "tcp");
    }

    private static string CreateTempDockerfile(FileInfo configFile)
    {
        var content = string.Format(CultureInfo.InvariantCulture, DockerFileContent, $"{YarpContainerImageTags.Image}:{YarpContainerImageTags.Tag}", "yarp.config");

        var tempContextPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempContextPath);
        var tmpDockerfile = Path.Combine(tempContextPath, "Dockerfile");
        File.Copy(configFile.FullName, Path.Combine(tempContextPath, "yarp.config"));

        File.WriteAllText(tmpDockerfile, content);

        return tempContextPath;
    }
}
