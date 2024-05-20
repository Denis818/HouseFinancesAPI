﻿using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Despesas.Base
{
    public abstract class BaseDespesaService : BaseAppService<Despesa, IDespesaRepository>
    {
        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IMembroRepository _membroRepository;
        protected readonly IGrupoDespesaRepository _grupoDespesaRepository;

        protected readonly CategoriaIdsDto _categoriaIds;
        protected (int IdJhon, int IdPeu) _membroId;
        protected int _grupoDespesaId = 0;

        protected IQueryable<Despesa> _queryDespesasPorGrupo;
        protected IQueryable<Despesa> _queryDespesasAllGrupo;

        public BaseDespesaService(IServiceProvider service)
            : base(service)
        {
            _membroRepository = service.GetRequiredService<IMembroRepository>();
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _grupoDespesaRepository = service.GetRequiredService<IGrupoDespesaRepository>();

            _membroId = _membroRepository.GetIdsJhonPeu();
            _categoriaIds = _categoriaRepository.GetCategoriaIds();

            _queryDespesasPorGrupo = GetDespesasByGroup();
            _queryDespesasAllGrupo = GetDespesasAllGroup();
        }

        public IQueryable<Despesa> GetDespesasByGroup()
        {
            _grupoDespesaId = (int)(_httpContext.Items["GrupoDespesaId"] ?? 0);

            var queryDespesas = _repository
                .Get(d => d.GrupoDespesaId == _grupoDespesaId)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoDespesa);

            return queryDespesas;
        }

        public IQueryable<Despesa> GetDespesasAllGroup()
        {
            var queryDespesas = _repository
                .Get()
                .Include(c => c.Categoria)
                .Include(c => c.GrupoDespesa);

            return queryDespesas;
        }
    }
}
