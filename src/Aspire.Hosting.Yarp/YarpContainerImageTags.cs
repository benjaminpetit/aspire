// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting.Yarp;

internal static class YarpContainerImageTags
{
    public const string Registry = "docker.io";

    public const string Image = "local/yarp:latest";
    //public const string Image = "dotnet/yarp:latest";

    public const string Tag = "latest";

    public const int Port = 5000;

    public const string ConfigFilePath = "/etc/yarp.config";
}
