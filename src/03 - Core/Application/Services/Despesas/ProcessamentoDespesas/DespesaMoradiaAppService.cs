using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas.ProcessamentoDespesas
{
    public class DespesaMoradiaAppService(IServiceProvider service)
        : BaseDespesaService(service),
            IDespesaMoradiaAppService
    {
        public async Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync()
        {
            var grupoListMembrosDespesa = await GetGrupoListMembrosDespesa();

            var custosDespesasMoradiaDto = await GetCustosDespesasMoradiaAsync();

            if(
                custosDespesasMoradiaDto.ContaDeLuz == 0
                && custosDespesasMoradiaDto.ParcelaApartamento == 0
                && grupoListMembrosDespesa.ListAluguel.Count <= 0
            )
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "de Moradia")
                );

                return new();
            }

            return new DetalhamentoDespesasMoradiaDto()
            {
                GrupoListMembrosDespesa = grupoListMembrosDespesa,

                CustosDespesasMoradia = custosDespesasMoradiaDto,

                DistribuicaoCustos = _despesaDomainServices.CalcularDistribuicaoCustosMoradiaAsync(
                    custosDespesasMoradiaDto.ParcelaApartamento,
                    custosDespesasMoradiaDto.ParcelaCaixa,
                    custosDespesasMoradiaDto.ContaDeLuz,
                    custosDespesasMoradiaDto.Condominio,
                    grupoListMembrosDespesa.ListMembroForaJhonPeu.Count,
                    grupoListMembrosDespesa.ListMembroForaJhon.Count
                )
            };
        }

        #region Metodos de Suporte
        private async Task<CustosDespesasMoradiaDto> GetCustosDespesasMoradiaAsync()
        {
            var listAluguel = ListDespesasRecentes.Where(d =>
                d.CategoriaId == _categoriaIds.IdAluguel
            );

            Despesa parcelaApartamento = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefaultAsync();

            Despesa parcelaCaixa = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefaultAsync();

            Despesa contaDeLuz = await ListDespesasRecentes
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdContaDeLuz)
                .FirstOrDefaultAsync();

            Despesa condominio = await ListDespesasRecentes
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdCondominio)
                .FirstOrDefaultAsync();

            return new CustosDespesasMoradiaDto()
            {
                Condominio = condominio?.Preco ?? 0,
                ContaDeLuz = contaDeLuz?.Preco ?? 0,
                ParcelaCaixa = parcelaCaixa?.Preco ?? 0,
                ParcelaApartamento = parcelaApartamento?.Preco ?? 0,
            };
        }

        private async Task<GrupoListMembrosDespesaDto> GetGrupoListMembrosDespesa()
        {
            List<Membro> listMembroForaJhon = await _membroRepository
                .Get(m => m.Id != _membroId.IdJhon)
                .ToListAsync();

            List<Membro> listMembroForaJhonPeu = listMembroForaJhon
                .Where(m => m.Id != _membroId.IdPeu)
                .ToList();

            List<Despesa> listAluguel = await ListDespesasRecentes
                .Where(d => d.CategoriaId == _categoriaIds.IdAluguel)
                .ToListAsync();

            return new GrupoListMembrosDespesaDto()
            {
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhon,
                ListMembroForaJhonPeu = listMembroForaJhonPeu
            };
        }
        #endregion
    }
}
