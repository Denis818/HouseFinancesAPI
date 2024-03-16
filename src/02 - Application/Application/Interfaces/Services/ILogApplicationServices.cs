using Domain.Enumeradores;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services
{
    public interface ILogApplicationServices
    {
        Task RegisterLog(EnumTypeLog typeLog, HttpContext context, 
            ObjectResult objectResult = null, Exception exception = null);
    }
}