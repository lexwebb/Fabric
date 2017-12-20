using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fabric.Server
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            dynamic model = new ExpandoObject();

            model.message = exception.Message;
            model.detail = exception.Message + (exception.InnerException == null ? "" : " | " + exception.InnerException.Message);
#if DEBUG
            model.stackTrace = exception.StackTrace;
#endif

            if (exception is HttpException)
            {
                model.detail = (exception as HttpException).ToString();
                context.HttpContext.Response.StatusCode = (int)(exception as HttpException).StatusCode;
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
            }
            context.Result = new JsonResult(model);
        }
    }
}
