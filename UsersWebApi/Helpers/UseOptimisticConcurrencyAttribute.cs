﻿namespace UsersWebApi.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using UsersWebApi.Services;


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class UseOptimisticConcurrencyAttribute : TypeFilterAttribute
{

    public UseOptimisticConcurrencyAttribute() : base(typeof(UseOptimisticConcurrencyFilter))
    {
    }

    private sealed class UseOptimisticConcurrencyFilter : IActionFilter
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
                        changeContext.RowVersion = Convert.FromBase64String(context.HttpContext.Request.Headers[MATCH_HEADER]);
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
            if (changeContext.RowVersion != null && context.HttpContext.Request.Method.Equals(HttpMethod.Get.Method))
            {
                context.HttpContext.Response.Headers.Add(ETAG_HEADER, Convert.ToBase64String(changeContext.RowVersion));
            }

            if (context.Exception is DbUpdateConcurrencyException)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
                context.ExceptionHandled = true;
            }

        }
    }
}

