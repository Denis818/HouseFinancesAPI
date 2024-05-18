using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Despesas.Base
{
    public abstract class BaseDespesaService : BaseAppService<Despesa, IDespesaRepository>
    {
        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IMembroRepository _membroRepository;
        protected readonly IDistributedCache _cache;

        protected readonly CategoriaIdsDto _categoriaIds;
        protected (int IdJhon, int IdPeu) _membroId;
        protected int _grupoDespesaId = 0;

        protected List<Despesa> ListDespesasPorGrupo { get; private set; }

        public BaseDespesaService(IServiceProvider service)
            : base(service)
        {
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _membroRepository = service.GetRequiredService<IMembroRepository>();
            _cache = service.GetRequiredService<IDistributedCache>();

            _categoriaIds = _categoriaRepository.GetCategoriaIds();
            _membroId = _membroRepository.GetIdsJhonPeu();

            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync() =>
            ListDespesasPorGrupo = await GetDespesasByGroupAsync();

        public async Task<List<Despesa>> GetDespesasByGroupAsync()
        {
            _grupoDespesaId = (int)(_httpContext.Items["GrupoDespesaId"] ?? 0);

            //var cacheKey = $"Despesas_{_grupoDespesaId}";

            //var cachedDespesas = await _cache.GetStringAsync(cacheKey);
            //if (!string.IsNullOrEmpty(cachedDespesas))
            //{
            //    return JsonSerializer.Deserialize<List<Despesa>>(cachedDespesas);
            //}

            var despesas = await _repository.GetDespesasUsingMySqlConnector(_grupoDespesaId);

            //var options = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            //};
            //await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(despesas), options);

            return despesas;
        }
    }

    public class DespesaDtoo
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public double Total { get; set; }
        public int GrupoDespesaId { get; set; }
        public string GrupoDespesaNome { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; }
    }
}
