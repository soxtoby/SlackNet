using System;
using Microsoft.AspNetCore.Http;

namespace SlackNet.AspNetCore;

class HttpContextServiceProviderAccessor : IRequestServiceProviderAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextServiceProviderAccessor(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public IServiceProvider ServiceProvider => _httpContextAccessor.HttpContext.RequestServices;
}