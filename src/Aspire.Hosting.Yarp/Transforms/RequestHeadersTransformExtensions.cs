// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting;

/// <summary>
/// Extensions for adding request header transforms.
/// </summary>
public static class RequestHeadersTransformExtensions
{
    internal static class RequestHeadersTransformFactory
    {
        internal const string RequestHeadersCopyKey = "RequestHeadersCopy";
        internal const string RequestHeaderOriginalHostKey = "RequestHeaderOriginalHost";
        internal const string RequestHeaderKey = "RequestHeader";
        internal const string RequestHeaderRouteValueKey = "RequestHeaderRouteValue";
        internal const string RequestHeaderRemoveKey = "RequestHeaderRemove";
        internal const string RequestHeadersAllowedKey = "RequestHeadersAllowed";
        internal const string AppendKey = "Append";
        internal const string SetKey = "Set";
    }

    /// <summary>
    /// Clones the route and adds the transform which will enable or suppress copying request headers to the proxy request.
    /// </summary>
    public static RouteTransformsBuilder WithTransformCopyRequestHeaders(this RouteTransformsBuilder builder, bool copy = true)
    {
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeadersCopyKey] = copy ? bool.TrueString : bool.FalseString;
        });
    }

    /// <summary>
    /// Clones the route and adds the transform which will copy the incoming request Host header to the proxy request.
    /// </summary>
    public static RouteTransformsBuilder WithTransformUseOriginalHostHeader(this RouteTransformsBuilder builder, bool useOriginal = true)
    {
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeaderOriginalHostKey] = useOriginal ? bool.TrueString : bool.FalseString;
        });
    }

    /// <summary>
    /// Clones the route and adds the transform which will append or set the request header.
    /// </summary>
    public static RouteTransformsBuilder WithTransformRequestHeader(this RouteTransformsBuilder builder, string headerName, string value, bool append = true)
    {
        var type = append ? RequestHeadersTransformFactory.AppendKey : RequestHeadersTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeaderKey] = headerName;
            transform[type] = value;
        });
    }

    /// <summary>
    /// Clones the route and adds the transform which will append or set the request header from a route value.
    /// </summary>
    public static RouteTransformsBuilder WithTransformRequestHeaderRouteValue(this RouteTransformsBuilder builder, string headerName, string routeValueKey, bool append = true)
    {
        var type = append ? RequestHeadersTransformFactory.AppendKey : RequestHeadersTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeaderRouteValueKey] = headerName;
            transform[type] = routeValueKey;
        });
    }

    /// <summary>
    /// Clones the route and adds the transform which will remove the request header.
    /// </summary>
    public static RouteTransformsBuilder WithTransformRequestHeaderRemove(this RouteTransformsBuilder builder, string headerName)
    {
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeaderRemoveKey] = headerName;
        });
    }

    /// <summary>
    /// Clones the route and adds the transform which will only copy the allowed request headers. Other transforms
    /// that modify or append to existing headers may be affected if not included in the allow list.
    /// </summary>
    public static RouteTransformsBuilder WithTransformRequestHeadersAllowed(this RouteTransformsBuilder builder, params string[] allowedHeaders)
    {
        return builder.WithTransform(transform =>
        {
            transform[RequestHeadersTransformFactory.RequestHeadersAllowedKey] = string.Join(';', allowedHeaders);
        });
    }
}
