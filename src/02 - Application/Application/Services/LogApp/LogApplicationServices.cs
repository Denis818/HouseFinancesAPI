using Application.Interfaces.Services.LogApp;
using Domain.Converters;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Application.Services.LogApp
{
    public class LogApplicationServices(ILogApplicationRepository LogRepository) : ILogApplicationServices
    {
        public async Task RegisterLog(EnumTypeLog typeLog,
            HttpContext context,
            ObjectResult objectResult,
            Exception ex)
        {
            var request = context.Request;
            string method = $"{request.Method} - StatusCode {context.Response.StatusCode}";
            string fullUrl = $"{request.Scheme}://{request.Host}{request.Path}";

            string content = JsonSerializer.Serialize(objectResult?.Value);

            var logEntry = new LogRequest
            {
                UserName = context.User.Identity.Name,
                TypeLog = typeLog.ToString(),
                Content = ReduzirString(content, 100),
                InclusionDate = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow),
                Method = method,
                Path = fullUrl,
                QueryString = request.QueryString.ToString(),
                StackTrace = "",
                ExceptionMessage = ""

            };

            if (typeLog is EnumTypeLog.Exception)
            {
                var message = $"InnerException: {ex?.InnerException} | Message: {ex?.Message}";

                logEntry.StackTrace = ReduzirString(ex?.StackTrace, 250);
                logEntry.ExceptionMessage = ReduzirString(message, 250);
            }

         //   await LogRepository.InsertAsync(logEntry);
        }

        private string ReduzirString(string message, int max)
        {
            if (message.IsNullOrEmpty()) message = "";

            return message.Substring(0, Math.Min(max, message.Length));
        }
    }
}
