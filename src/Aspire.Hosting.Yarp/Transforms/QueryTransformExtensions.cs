// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting;

/// <summary>
/// Extensions for adding query transforms.
/// </summary>
public static class QueryTransformExtensions
{
    internal static class QueryTransformFactory
    {
        internal const string QueryValueParameterKey = "QueryValueParameter";
        internal const string QueryRouteParameterKey = "QueryRouteParameter";
        internal const string QueryRemoveParameterKey = "QueryRemoveParameter";
        internal const string AppendKey = "Append";
        internal const string SetKey = "Set";
    }

    /// <summary>
    /// Adds the transform that will append or set the query parameter from the given value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformQueryValue(this RouteTransformsBuilder builder, string queryKey, string value, bool append = true)
    {
        var type = append ? QueryTransformFactory.AppendKey : QueryTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[QueryTransformFactory.QueryValueParameterKey] = queryKey;
            transform[type] = value;
        });
    }

    /// <summary>
    /// Adds the transform that will append or set the query parameter from a route value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformQueryRouteValue(this RouteTransformsBuilder builder, string queryKey, string routeValueKey, bool append = true)
    {
        var type = append ? QueryTransformFactory.AppendKey : QueryTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[QueryTransformFactory.QueryRouteParameterKey] = queryKey;
            transform[type] = routeValueKey;
        });
    }

    /// <summary>
    /// Adds the transform that will remove the given query key.
    /// </summary>
    public static RouteTransformsBuilder WithTransformQueryRemoveKey(this RouteTransformsBuilder builder, string queryKey)
    {
        return builder.WithTransform(transform =>
        {
            transform[QueryTransformFactory.QueryRemoveParameterKey] = queryKey;
        });
    }
}
