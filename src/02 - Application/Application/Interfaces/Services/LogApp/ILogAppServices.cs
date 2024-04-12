using Domain.Enumeradores;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services.LogApp
{
    public interface ILogAppServices
    {
        Task RegisterLog(
            EnumTypeLog typeLog,
            HttpContext context,
            ObjectResult objectResult = null,
            Exception exception = null
        );
    }
}
