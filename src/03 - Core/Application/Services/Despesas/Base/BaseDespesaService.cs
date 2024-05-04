using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Despesas.Base
{
    public abstract class BaseDespesaService : BaseAppService<Despesa, IDespesaRepository>
    {
        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IMembroRepository _membroRepository;
        protected readonly IDespesaDomainServices _despesaDomainServices;

        protected readonly CategoriaIdsDto _categoriaIds;

        protected (int IdJhon, int IdPeu) _membroId;
        protected IQueryable<Despesa> ListDespesasRecentes;

        public BaseDespesaService(IServiceProvider service)
            : base(service)
        {
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _membroRepository = service.GetRequiredService<IMembroRepository>();
            _despesaDomainServices = service.GetRequiredService<IDespesaDomainServices>();

            _categoriaIds = _categoriaRepository.GetCategoriaIds();
            _membroId = _membroRepository.GetIdsJhonPeu();

            ListDespesasRecentes = GetDespesasByGroup();
        }

        public IQueryable<Despesa> GetDespesasByGroup()
        {
            var queryDespesas = _repository.Get();
            string grupoDespesasIdHeader = _httpContext.Request.Headers["Grupo-Despesas-Id"];

            if(int.TryParse(grupoDespesasIdHeader, out int grupoDespesaId))
            {
                queryDespesas = queryDespesas
                    .Where(d => d.GrupoDespesaId == grupoDespesaId)
                    .Include(c => c.Categoria);

                return queryDespesas;
            }

            return queryDespesas;
        }
    }
}
