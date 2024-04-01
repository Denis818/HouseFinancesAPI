using Microsoft.AspNetCore.Hosting;

namespace Application.Configurations.Extensions.Help
{
    public static class HelpExtensios
    {
        public static decimal RoundTo(this decimal soucer, int decimalPlaces)
            => Math.Round(soucer, decimalPlaces);

        public static string RootPathCombine(this string soucer, IWebHostEnvironment env)
            => Path.Combine(env.WebRootPath, soucer);

        public static string ReadFileFromRootPath(this IWebHostEnvironment env, string soucer) 
            => File.ReadAllText(Path.Combine(env.WebRootPath, soucer));
    }
}
