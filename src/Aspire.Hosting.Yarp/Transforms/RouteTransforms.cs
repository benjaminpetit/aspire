// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting;

/// <summary>
/// Helper class to build a list of route transforms
/// </summary>
public class RouteTransformsBuilder
{
    private readonly List<Dictionary<string, string>> _transforms = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="createTransform"></param>
    /// <returns></returns>
    public RouteTransformsBuilder WithTransform(Action<Dictionary<string, string>> createTransform)
    {
        var transform = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        createTransform(transform);
        _transforms.Add(transform);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IReadOnlyDictionary<string, string>> Build()
    {
        return _transforms.ToArray();
    }
}
