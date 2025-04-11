using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Talabat.Core.IServices;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var response = await responseCacheService.GetCacheResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = result;
                return;
            }
            var excutedActionContext = await next.Invoke();
            if (excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);
            foreach(var (key, value) in request.Query.OrderBy(X => X.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
