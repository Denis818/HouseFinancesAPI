﻿using Application.Interfaces.Utilities;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Base;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Base
{
    public abstract class BaseAppService<TEntity, TIRepository>(IServiceProvider service)
        where TEntity : class, new()
        where TIRepository : class, IRepositoryBase<TEntity>
    {
        protected readonly IMapper _mapper = service.GetRequiredService<IMapper>();

        private readonly INotifier _notificador = service.GetRequiredService<INotifier>();

        protected readonly TIRepository _repository = service.GetRequiredService<TIRepository>();

        protected readonly HttpContext _httpContext = service
            .GetRequiredService<IHttpContextAccessor>()
            .HttpContext;

        public void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notificador.Notificar(tipo, message);

        protected bool Validator<TEntityDto>(TEntityDto entityDto)
        {
            var validator = service.GetService<IValidator<TEntityDto>>();

            ValidationResult results = validator.Validate(entityDto);

            if(!results.IsValid)
            {
                var groupedFailures = results
                    .Errors.GroupBy(failure => failure.PropertyName)
                    .Select(group => new
                    {
                        PropertyName = group.Key,
                        Errors = string.Join(" ", group.Select(err => err.ErrorMessage))
                    });

                foreach(var failure in groupedFailures)
                {
                    Notificar(EnumTipoNotificacao.Informacao, $"{failure.Errors}");
                }

                return true;
            }

            return false;
        }
    }
}
