using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ArvatoAssessment.Authorization
{
    public class AuthorizationAttribute : Attribute, IAsyncActionFilter
    {
        private const string AuthorizationHeaderName = "Authorization";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaderName, out var potentialKey))
            {
                context.Result = new ContentResult() { Content = HttpStatusCode.Forbidden.ToString(), StatusCode = (int)HttpStatusCode.Forbidden };
                return;
            }
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            //Get authorized clients tokens from config
            //for proper implementation should store and get client data in database
            IConfigurationSection myArraySection = config.GetSection("Authorization");
            var authorizedKeys = myArraySection.AsEnumerable().Select(_ => _.Value).ToList();

            if (!authorizedKeys.Contains(potentialKey))
            {
                context.Result = new ContentResult() { Content = HttpStatusCode.Forbidden.ToString(), StatusCode = (int)HttpStatusCode.Forbidden };
                return;
            }
            await next();
        }
    }
}
