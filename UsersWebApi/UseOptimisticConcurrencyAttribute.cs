using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using UsersWebApi.Services;

namespace UsersWebApi
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UseOptimisticConcurrencyAttribute : TypeFilterAttribute
    {

        public UseOptimisticConcurrencyAttribute() : base(typeof(UseOptimisticConcurrencyFilter))
        {
        }

        private class UseOptimisticConcurrencyFilter : IActionFilter
        {
            private readonly ChangeContext changeContext;

            const string ETAG_HEADER = "ETag";
            const string MATCH_HEADER = "If-Match";

            public UseOptimisticConcurrencyFilter(ChangeContext changeContext)
            {
                this.changeContext = changeContext;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var method = context.HttpContext.Request.Method;
                if (method.Equals(HttpMethod.Post.Method) || method.Equals(HttpMethod.Put.Method))
                {
                    if (context.HttpContext.Request.Headers.ContainsKey(MATCH_HEADER))
                    {
                        try
                        {
                            changeContext.Timestamp = Convert.FromBase64String(context.HttpContext.Request.Headers[MATCH_HEADER]);
                        }
                        catch (FormatException)
                        {
                            context.Result = new StatusCodeResult(StatusCodes.Status428PreconditionRequired);
                        }
                    }
                    else
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status428PreconditionRequired);
                    }
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                if (changeContext.Timestamp != null)
                {
                    context.HttpContext.Response.Headers.Add(ETAG_HEADER, Convert.ToBase64String(changeContext.Timestamp));
                }

                if (context.Exception is DbUpdateConcurrencyException)
                {
                    context.Result = new ConflictResult();
                    context.ExceptionHandled = true;
                }
                else if (context.Exception is PreconditionFailedException)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
                    context.ExceptionHandled = true;
                }

            }
        }
    }
}
