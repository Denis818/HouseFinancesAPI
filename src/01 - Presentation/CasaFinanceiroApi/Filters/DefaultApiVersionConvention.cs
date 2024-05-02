using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CasaFinanceiroApi.Filters
{
    public class DefaultApiVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var versionAttribute = controller
                .Attributes.OfType<ApiVersionAttribute>()
                .FirstOrDefault();

            if(versionAttribute != null)
            {
                var version = versionAttribute.Versions.FirstOrDefault();
                if(version != null)
                {
                    controller.ApiExplorer.GroupName = version.MajorVersion.ToString();
                }
            }
        }
    }
}
