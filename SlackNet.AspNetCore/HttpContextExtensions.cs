using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SlackNet.AspNetCore
{
    static class HttpContextExtensions
    {
        public static async Task<HttpResponse> Respond(this HttpContext context, HttpStatusCode status, string contentType = null, string body = null)
        {
            context.Response.StatusCode = (int)status;
            if (contentType != null)
                context.Response.ContentType = contentType;
            if (body != null)
                await context.Response.WriteAsync(body).ConfigureAwait(false);
            return context.Response;
        }
    }
}