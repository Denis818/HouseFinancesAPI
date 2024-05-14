using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ModelState.Interface
{
    public interface IModelStateValidator
    {
        bool ValidarModelState(ActionExecutingContext context);
    }
}