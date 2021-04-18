using Microsoft.AspNetCore.Http;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore
{
    class AspNetCoreServiceProviderSlackRequestListener : IServiceProviderSlackRequestListener
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AspNetCoreServiceProviderSlackRequestListener(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public void OnRequestBegin(SlackRequestContext context) =>
            context.SetServiceProvider(_httpContextAccessor.HttpContext.RequestServices);
    }
}