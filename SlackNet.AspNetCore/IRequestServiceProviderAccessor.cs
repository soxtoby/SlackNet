#nullable enable
using System;

namespace SlackNet.AspNetCore;

/// <summary>
/// Provides access to the service provider scoped to the current HTTP request, if available.
/// </summary>
public interface IRequestServiceProviderAccessor
{
    IServiceProvider? ServiceProvider { get; }
}