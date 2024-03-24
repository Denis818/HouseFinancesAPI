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
    public abstract class ServiceAppBase<TEntity, TEntityDto, TIRepository>(IServiceProvider service)
      where TEntity : class, new()
      where TIRepository : class, IRepositoryBase<TEntity>
    {
        private readonly IMapper _mapper = service.GetRequiredService<IMapper>();
        private readonly INotificador _notificador = service.GetRequiredService<INotificador>();
        private readonly IValidator<TEntityDto> _validator = service.GetRequiredService<IValidator<TEntityDto>>();

        protected readonly TIRepository _repository = service.GetRequiredService<TIRepository>();
        protected readonly HttpContext _context = service.GetRequiredService<IHttpContextAccessor>().HttpContext;

        public void MapDtoToModel(TEntityDto entityDto, TEntity entity)
          => _mapper.Map(entityDto, entity);

        public TEntityDto MapToDto(TEntity entity)
            => _mapper.Map<TEntityDto>(entity);

        public TEntity MapToModel(TEntityDto entityDto)
            => _mapper.Map<TEntity>(entityDto);

        public IEnumerable<TEntityDto> MapToListDto(IEnumerable<TEntity> entityDto)
            => _mapper.Map<IEnumerable<TEntityDto>>(entityDto);

        public void Notificar(EnumTipoNotificacao tipo, string message)
            => _notificador.Add(new Notificacao(message, tipo));

        public bool Validator(TEntityDto entityDto)
        {
            ValidationResult results = _validator.Validate(entityDto);

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
                    Notificar(EnumTipoNotificacao.ClientError, $"{failure.PropertyName}: {failure.Errors}");
                }

                return true;
            }

            return false;
        }
    }
}
