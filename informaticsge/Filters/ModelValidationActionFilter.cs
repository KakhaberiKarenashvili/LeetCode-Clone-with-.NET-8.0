using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace informaticsge.Filters;

public class ModelValidationActionFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var modelState = context.ModelState;
        if (!modelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(modelState);
        }
    }
}