// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting;

/// <summary>
/// Extensions for modifying the request method.
/// </summary>
public static class HttpMethodTransformsBuilderExtensions
{
    internal static class HttpMethodTransformFactory
    {
        internal const string HttpMethodChangeKey = "HttpMethodChange";
        internal const string SetKey = "Set";
    }

    /// <summary>
    /// Adds the transform that will replace the HTTP method if it matches.
    /// </summary>
    public static RouteTransformsBuilder WithTransformHttpMethodChange(this RouteTransformsBuilder builder, string fromHttpMethod, string toHttpMethod)
    {
        return builder.WithTransform(transform =>
        {
            transform[HttpMethodTransformFactory.HttpMethodChangeKey] = fromHttpMethod;
            transform[HttpMethodTransformFactory.SetKey] = toHttpMethod;
        });
    }
}
