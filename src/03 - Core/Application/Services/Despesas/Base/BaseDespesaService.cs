using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Application.Services.Despesas.Base
{
    public abstract class BaseDespesaService : BaseAppService<Despesa, IDespesaRepository>
    {
        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IMembroRepository _membroRepository;
        protected readonly IDespesaDomainServices _despesaDomainServices;

        protected readonly CategoriaIdsDto _categoriaIds;

        protected (int IdJhon, int IdPeu) _membroId;
        protected readonly IQueryable<Despesa> ListDespesasRecentes;

        public BaseDespesaService(IServiceProvider service)
            : base(service)
        {
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _membroRepository = service.GetRequiredService<IMembroRepository>();
            _despesaDomainServices = service.GetRequiredService<IDespesaDomainServices>();

            _categoriaIds = _categoriaRepository.GetCategoriaIds();
            _membroId = _membroRepository.GetIdsJhonPeu();

            ListDespesasRecentes = GetDespesasMaisRecentes();
        }

        protected IQueryable<Despesa> GetDespesasMaisRecentes(
            Expression<Func<Despesa, bool>> filter = null
        )
        {
            var dataMaisRecente = _repository
                .Get()
                .OrderByDescending(d => d.DataCompra)
                .Select(d => d.DataCompra)
                .FirstOrDefault();

            var inicioDoMes = new DateTime(dataMaisRecente.Year, dataMaisRecente.Month, 1);
            var fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            IQueryable<Despesa> query = _repository
                .Get(d => d.DataCompra.Date >= inicioDoMes && d.DataCompra.Date <= fimDoMes)
                .Include(c => c.Categoria);

            var t = _repository.Get().Select(d => d.DataCompra).ToList();

            if(filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }
    }
}
