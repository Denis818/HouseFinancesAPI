using Microsoft.AspNetCore.Builder;

namespace Application.Configurations.Extensions.Help
{
    public static class HelpExtensios
    {
        public static decimal RoundTo(this decimal soucer, int decimalPlaces)
            => Math.Round(soucer, decimalPlaces);

        public static string ReadFileFromRootPath(this WebApplication app, string soucer) 
            => File.ReadAllText(Path.Combine(app.Environment.WebRootPath, soucer));
    }
}
