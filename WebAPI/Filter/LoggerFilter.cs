using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filter
{
    public class LoggerFilter : IActionFilter
    {
        //Выполняется до вызова метода контроллера
        public void OnActionExecuting(ActionExecutingContext context)
        {
            ILogger _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
            string content = $"({context.HttpContext.Request.Method}):({context.HttpContext.Request.Path}):({context.HttpContext.Request.PathBase.ToString()}):({context.HttpContext.Request.Body.ToString()})";
            _logger.LogInformation(content);
        }
        //Выполняется после вызова метода контроллера
        public void OnActionExecuted(ActionExecutedContext context)
        {
            ILogger _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
            string content = $"({context.HttpContext.Request.Method}):({context.HttpContext.Request.Path}):({context.HttpContext.Request.PathBase.ToString()}):({context.HttpContext.Request.Body.ToString()})";
            _logger.LogInformation(content);
        }
    }
}
