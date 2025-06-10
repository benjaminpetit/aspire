// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Yarp.ReverseProxy.Transforms;

namespace Aspire.Hosting;

/// <summary>
/// Extensions for adding response header and trailer transforms.
/// </summary>
public static class ResponseTransformExtensions
{
    internal static class ResponseTransformFactory
    {
        internal const string ResponseHeadersCopyKey = "ResponseHeadersCopy";
        internal const string ResponseTrailersCopyKey = "ResponseTrailersCopy";
        internal const string ResponseHeaderKey = "ResponseHeader";
        internal const string ResponseTrailerKey = "ResponseTrailer";
        internal const string ResponseHeaderRemoveKey = "ResponseHeaderRemove";
        internal const string ResponseTrailerRemoveKey = "ResponseTrailerRemove";
        internal const string ResponseHeadersAllowedKey = "ResponseHeadersAllowed";
        internal const string ResponseTrailersAllowedKey = "ResponseTrailersAllowed";
        internal const string WhenKey = "When";
        internal const string AppendKey = "Append";
        internal const string SetKey = "Set";
    }

    /// <summary>
    /// Clones the builder and adds the transform which will enable or suppress copying response headers to the client response.
    /// </summary>
    public static RouteTransformsBuilder WithTransformCopyResponseHeaders(this RouteTransformsBuilder builder, bool copy = true)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseHeadersCopyKey] = copy ? bool.TrueString : bool.FalseString;
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will enable or suppress copying response trailers to the client response.
    /// </summary>
    public static RouteTransformsBuilder WithTransformCopyResponseTrailers(this RouteTransformsBuilder builder, bool copy = true)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseTrailersCopyKey] = copy ? bool.TrueString : bool.FalseString;
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will append or set the response header.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseHeader(this RouteTransformsBuilder builder, string headerName, string value, bool append = true, ResponseCondition condition = ResponseCondition.Success)
    {
        var type = append ? ResponseTransformFactory.AppendKey : ResponseTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseHeaderKey] = headerName;
            transform[type] = value;
            transform[ResponseTransformFactory.WhenKey] = condition.ToString();
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will remove the response header.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseHeaderRemove(this RouteTransformsBuilder builder, string headerName, ResponseCondition condition = ResponseCondition.Success)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseHeaderRemoveKey] = headerName;
            transform[ResponseTransformFactory.WhenKey] = condition.ToString();
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will only copy the allowed response headers. Other transforms
    /// that modify or append to existing headers may be affected if not included in the allow list.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseHeadersAllowed(this RouteTransformsBuilder builder, params string[] allowedHeaders)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseHeadersAllowedKey] = string.Join(';', allowedHeaders);
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will append or set the response trailer.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseTrailer(this RouteTransformsBuilder builder, string headerName, string value, bool append = true, ResponseCondition condition = ResponseCondition.Success)
    {
        var type = append ? ResponseTransformFactory.AppendKey : ResponseTransformFactory.SetKey;
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseTrailerKey] = headerName;
            transform[type] = value;
            transform[ResponseTransformFactory.WhenKey] = condition.ToString();
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will remove the response trailer.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseTrailerRemove(this RouteTransformsBuilder builder, string headerName, ResponseCondition condition = ResponseCondition.Success)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseTrailerRemoveKey] = headerName;
            transform[ResponseTransformFactory.WhenKey] = condition.ToString();
        });
    }

    /// <summary>
    /// Clones the builder and adds the transform which will only copy the allowed response trailers. Other transforms
    /// that modify or append to existing trailers may be affected if not included in the allow list.
    /// </summary>
    public static RouteTransformsBuilder WithTransformResponseTrailersAllowed(this RouteTransformsBuilder builder, params string[] allowedHeaders)
    {
        return builder.WithTransform(transform =>
        {
            transform[ResponseTransformFactory.ResponseTrailersAllowedKey] = string.Join(';', allowedHeaders);
        });
    }
}
