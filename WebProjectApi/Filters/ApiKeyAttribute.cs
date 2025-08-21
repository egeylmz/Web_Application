using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebProjectApi.Filters 
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var expected = ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["ApiKey"];
            var provided = ctx.HttpContext.Request.Headers["X-API-KEY"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(expected) || provided != expected)
            {
                ctx.Result = new UnauthorizedObjectResult("API key missing/invalid");
                return;
            }
            await next();
        }
    }
}
