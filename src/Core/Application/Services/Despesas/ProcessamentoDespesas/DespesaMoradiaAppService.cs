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

            var custosDespesasMoradiaDto = await GetCustosDespesasMoradiaAsync();

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
                        ValorParaDoPeu = 300, // 300 reais do aluguel é fixo.
                    }
                };
            }

            var custosMoradiaDto = new CustosDespesasMoradiaDto
            {
                ParcelaApartamento = custosDespesasMoradiaDto.ParcelaApartamento,
                ParcelaCaixa = custosDespesasMoradiaDto.ParcelaCaixa,
                ContaDeLuz = custosDespesasMoradiaDto.ContaDeLuz,
                Condominio = custosDespesasMoradiaDto.Condominio,
                MembrosForaJhonPeuCount = grupoListMembrosDespesa.ListMembroForaJhonPeu.Count,
                MembrosForaJhonCount = grupoListMembrosDespesa.ListMembroForaJhon.Count
            };

            return new DetalhamentoDespesasMoradiaDto()
            {
                GrupoListMembrosDespesa = grupoListMembrosDespesa,

                CustosDespesasMoradia = custosDespesasMoradiaDto,

                DistribuicaoCustos = _despesaDomainServices.CalcularDistribuicaoCustosMoradia(
                    custosMoradiaDto
                )
            };
        }

        #region Metodos de Suporte
        private async Task<CustosDespesasMoradiaDto> GetCustosDespesasMoradiaAsync()
        {
            var listAluguel = _queryDespesasPorGrupo.Where(d =>
                d.CategoriaId == _categoriaIds.IdAluguel
            );

            Despesa parcelaApartamento = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefaultAsync();

            Despesa parcelaCaixa = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefaultAsync();

            Despesa contaDeLuz = await _queryDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdContaDeLuz)
                .FirstOrDefaultAsync();

            Despesa condominio = await _queryDespesasPorGrupo
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

            List<Despesa> listAluguel = await _queryDespesasPorGrupo
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
