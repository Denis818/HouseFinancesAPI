using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Presentation.Configurations.Filters
{
    public class ApiVersioningFilter : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var version = controller
                .Attributes.OfType<ApiVersionAttribute>()
                .FirstOrDefault();

            if(version != null)
            {
                var t = $"v{version.Versions.FirstOrDefault()}";
                controller.ApiExplorer.GroupName = t;
            }
        }
    }
}
