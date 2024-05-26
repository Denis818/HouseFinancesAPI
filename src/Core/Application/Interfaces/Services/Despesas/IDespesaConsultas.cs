using Application.Services.Despesas.Operacoes;
using Domain.Dtos.Despesas.Consultas;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Utilities;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultas
    {
        Task<List<SugestaoEconomiaInfoDto>> GetSugestoesEconomiaPorGrupoAsync();
        Task<IEnumerable<MediaPorFornecedorDto>> MediaDespesasPorFornecedorAsync(
            int paginaAtual,
            int itensPorPagina
        );
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
        Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync();
    }
}
