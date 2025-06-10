// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Yarp.ReverseProxy.Transforms;

namespace Aspire.Hosting;

/// <summary>
/// Extensions for adding forwarded header transforms.
/// </summary>
public static class ForwardedTransformsBuilderExtensions
{
    internal static class ForwardedTransformFactory
    {
        internal const string XForwardedKey = "X-Forwarded";
        internal const string DefaultXForwardedPrefix = "X-Forwarded-";
        internal const string ForwardedKey = "Forwarded";
        internal const string ActionKey = "Action";
        internal const string HeaderPrefixKey = "HeaderPrefix";
        internal const string ForKey = "For";
        internal const string ByKey = "By";
        internal const string HostKey = "Host";
        internal const string ProtoKey = "Proto";
        internal const string PrefixKey = "Prefix";
        internal const string ForFormatKey = "ForFormat";
        internal const string ByFormatKey = "ByFormat";
        internal const string ClientCertKey = "ClientCert";
    }

    /// <summary>
    /// Adds the transform which will add X-Forwarded-* headers.
    /// </summary>
    public static RouteTransformsBuilder WithTransformXForwarded(
        this RouteTransformsBuilder builder,
        string headerPrefix = "X-Forwarded-",
        ForwardedTransformActions xDefault = ForwardedTransformActions.Set,
        ForwardedTransformActions? xFor = null,
        ForwardedTransformActions? xHost = null,
        ForwardedTransformActions? xProto = null,
        ForwardedTransformActions? xPrefix = null)
    {
        return builder.WithTransform(transform =>
        {
            transform[ForwardedTransformFactory.XForwardedKey] = xDefault.ToString();

            if (xFor is not null)
            {
                transform[ForwardedTransformFactory.ForKey] = xFor.Value.ToString();
            }

            if (xPrefix is not null)
            {
                transform[ForwardedTransformFactory.PrefixKey] = xPrefix.Value.ToString();
            }

            if (xHost is not null)
            {
                transform[ForwardedTransformFactory.HostKey] = xHost.Value.ToString();
            }

            if (xProto is not null)
            {
                transform[ForwardedTransformFactory.ProtoKey] = xProto.Value.ToString();
            }

            transform[ForwardedTransformFactory.HeaderPrefixKey] = headerPrefix;
        });
    }

    /// <summary>
    /// Adds the transform which will add the Forwarded header as defined by [RFC 7239](https://tools.ietf.org/html/rfc7239).
    /// </summary>
    public static RouteTransformsBuilder WithTransformForwarded(
        this RouteTransformsBuilder builder,
        bool useHost = true,
        bool useProto = true,
        NodeFormat forFormat = NodeFormat.Random,
        NodeFormat byFormat = NodeFormat.Random,
        ForwardedTransformActions action = ForwardedTransformActions.Set)
    {
        var headers = new List<string>();

        if (forFormat != NodeFormat.None)
        {
            headers.Add(ForwardedTransformFactory.ForKey);
        }

        if (byFormat != NodeFormat.None)
        {
            headers.Add(ForwardedTransformFactory.ByKey);
        }

        if (useHost)
        {
            headers.Add(ForwardedTransformFactory.HostKey);
        }

        if (useProto)
        {
            headers.Add(ForwardedTransformFactory.ProtoKey);
        }

        return builder.WithTransform(transform =>
        {
            transform[ForwardedTransformFactory.ForwardedKey] = string.Join(',', headers);
            transform[ForwardedTransformFactory.ActionKey] = action.ToString();

            if (forFormat != NodeFormat.None)
            {
                transform.Add(ForwardedTransformFactory.ForFormatKey, forFormat.ToString());
            }

            if (byFormat != NodeFormat.None)
            {
                transform.Add(ForwardedTransformFactory.ByFormatKey, byFormat.ToString());
            }
        });
    }

    /// <summary>
    /// Adds the transform which will set the given header with the Base64 encoded client certificate.
    /// </summary>
    public static RouteTransformsBuilder WithTransformClientCertHeader(this RouteTransformsBuilder builder, string headerName)
    {
        return builder.WithTransform(transform =>
        {
            transform[ForwardedTransformFactory.ClientCertKey] = headerName;
        });
    }
}
