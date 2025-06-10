// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Aspire.Hosting;

/// <summary>
/// Extensions for adding path transforms.
/// </summary>
public static class PathTransformsBuilderExtensions
{
    internal static class PathTransformFactory
    {
        internal const string PathSetKey = "PathSet";
        internal const string PathPrefixKey = "PathPrefix";
        internal const string PathRemovePrefixKey = "PathRemovePrefix";
        internal const string PathPatternKey = "PathPattern";
    }

    /// <summary>
    /// Adds the transform which sets the request path with the given value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformPathSet(this RouteTransformsBuilder builder, PathString path)
    {
        return builder.WithTransform(transform =>
        {
            transform[PathTransformFactory.PathSetKey] = path.Value!;
        });
    }

    /// <summary>
    /// Adds the transform which will prefix the request path with the given value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformPathPrefix(this RouteTransformsBuilder builder, PathString path)
    {
        return builder.WithTransform(transform =>
        {
            transform[PathTransformFactory.PathPrefixKey] = path.Value!;
        });
    }

    /// <summary>
    /// Adds the transform which will remove the matching prefix from the request path.
    /// </summary>
    public static RouteTransformsBuilder WithTransformPathRemovePrefix(this RouteTransformsBuilder builder, PathString path)
    {
        return builder.WithTransform(transform =>
        {
            transform[PathTransformFactory.PathRemovePrefixKey] = path.Value!;
        });
    }

    /// <summary>
    /// Adds the transform which will set the request path with the given value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformPathRouteValues(this RouteTransformsBuilder builder, [StringSyntax("Route")] PathString path)
    {
        return builder.WithTransform(transform =>
        {
            transform[PathTransformFactory.PathPatternKey] = path.Value!;
        });
    }
}
