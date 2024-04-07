using Application.Interfaces.Utility;
using Application.Utilities;
using AutoMapper;
using Domain.Enumeradores;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Base
{
    public abstract class BaseService<TEntity, TIRepository>(IServiceProvider service)
      where TEntity : class, new()
      where TIRepository : class, IRepositoryBase<TEntity>
    {
        protected readonly IMapper _mapper = 
            service.GetRequiredService<IMapper>();

        private readonly INotificador _notificador =
            service.GetRequiredService<INotificador>();

        protected readonly TIRepository _repository =         
            service.GetRequiredService<TIRepository>();

        protected readonly HttpContext _httpContext = 
            service.GetRequiredService<IHttpContextAccessor>().HttpContext;

   
        public void Notificar(EnumTipoNotificacao tipo, string message)
            => _notificador.Notificar(tipo, message);

        public bool Validator<TEntityDto>(TEntityDto entityDto)
        {
            var validator = service.GetService<IValidator<TEntityDto>>();

            ValidationResult results = validator.Validate(entityDto);

            if (!results.IsValid)
            {
                var groupedFailures = results.Errors
                                             .GroupBy(failure => failure.PropertyName)
                                             .Select(group => new
                                             {
                                                 PropertyName = group.Key,
                                                 Errors = string.Join(" ", group.Select(err => err.ErrorMessage))
                                             });

                foreach (var failure in groupedFailures)
                {
                    _notificador.Notificar(EnumTipoNotificacao.Informacao, $"{failure.PropertyName}: {failure.Errors}");
                }

                return true;
            }

            return false;
        }
    }
}
