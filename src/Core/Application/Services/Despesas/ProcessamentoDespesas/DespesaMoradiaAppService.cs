using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Enumeradores;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas.ProcessamentoDespesas
{
    public class DespesaMoradiaAppService(
        IServiceProvider service,
        IDespesaDomainServices _despesaDomainServices
    ) : BaseDespesaService(service), IDespesaMoradiaAppService
    {
        public async Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync()
        {
            var grupoListMembrosDespesa = await GetGrupoListMembrosDespesa();

            var custosDespesasMoradiaDto = GetCustosDespesasMoradia();

            if (
                custosDespesasMoradiaDto.ContaDeLuz == 0
                && custosDespesasMoradiaDto.ParcelaApartamento == 0
                && grupoListMembrosDespesa.ListAluguel.Count <= 0
            )
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "de Moradia")
                );

                return new DetalhamentoDespesasMoradiaDto
                {
                    GrupoListMembrosDespesa = grupoListMembrosDespesa,
                    CustosDespesasMoradia = custosDespesasMoradiaDto,
                    DistribuicaoCustos = new DistribuicaoCustosMoradiaDto
                    {
                        ValorParaDoPeu = 300, // 300 reais do aluguel e fixo.
                    }
                };
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
        private CustosDespesasMoradiaDto GetCustosDespesasMoradia()
        {
            var listAluguel = ListDespesasPorGrupo.Where(d =>
                d.CategoriaId == _categoriaIds.IdAluguel
            );

            Despesa parcelaApartamento = listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefault();

            Despesa parcelaCaixa = listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefault();

            Despesa contaDeLuz = ListDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdContaDeLuz)
                .FirstOrDefault();

            Despesa condominio = ListDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdCondominio)
                .FirstOrDefault();

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

            List<Despesa> listAluguel = ListDespesasPorGrupo
                .Where(d => d.CategoriaId == _categoriaIds.IdAluguel)
                .ToList();

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
