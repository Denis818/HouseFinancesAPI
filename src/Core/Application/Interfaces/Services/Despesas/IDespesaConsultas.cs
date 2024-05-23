using Application.Services.Despesas.Operacoes;
using Application.Utilities;
using Domain.Dtos.Despesas.Consultas;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultas
    {
        Task<IEnumerable<SugestaoEconomiaDespesa>> SugerirOtimizacaoDeDespesasAsync();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
        Task<Despesa> GetByIdAsync(int id);
        Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync();
        Task<PagedResult<Despesa>> GetListDespesasAllGroups(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        );
        Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        );
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
    }
}
