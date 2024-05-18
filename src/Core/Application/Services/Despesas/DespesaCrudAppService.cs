﻿using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Application.Utilities;
using Domain.Converters.DatesTimes;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas
{
    public class DespesaCrudAppService(
        IServiceProvider service,
        IGrupoDespesaRepository _grupoDespesaRepository
    ) : BaseDespesaService(service), IDespesaCrudAppService
    {
        #region CRUD
        public async Task<Despesa> GetByIdAsync(int id)
        {
            var despesa = await _repository
                .Get(despesa => despesa.Id == id)
                .Include(x => x.Categoria)
                .Include(x => x.GrupoDespesa)
                .FirstOrDefaultAsync();

            return despesa;
        }

        public async Task<PagedResult<Despesa>> GetListDespesas(
            string filterItem,
            int paginaAtual,
            int itensPorPagina
        )
        {
            if (string.IsNullOrEmpty(filterItem))
            {
                return await GetAllDespesas(paginaAtual, itensPorPagina);
            }

            var query = ListDespesasPorGrupo
                .Where(despesa => despesa.Item.ToLower().Contains(filterItem.ToLower()))
                .OrderByDescending(d => d.DataCompra);

            var listaPaginada = await Pagination.PaginateResultAsync(
                query.AsQueryable(),
                paginaAtual,
                itensPorPagina
            );

            return listaPaginada;
        }

        private async Task<PagedResult<Despesa>> GetAllDespesas(int paginaAtual, int itensPorPagina)
        {
            var queryAll = ListDespesasPorGrupo.OrderByDescending(d => d.DataCompra);

            var despesas = await Pagination.PaginateResultAsync(
                queryAll.AsQueryable(),
                paginaAtual,
                itensPorPagina
            );

            if (despesas.TotalItens == 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
            }

            return despesas;
        }

        public async Task<Despesa> InsertAsync(DespesaDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            if (!await ValidarDespesaAsync(despesaDto))
                return null;

            var despesa = _mapper.Map<Despesa>(despesaDto);

            despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);
            despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow);

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task<IEnumerable<Despesa>> InsertRangeAsync(
            IAsyncEnumerable<DespesaDto> listDespesasDto
        )
        {
            int totalRecebido = 0;
            var despesasParaInserir = new List<Despesa>();

            await foreach (var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if (Validator(despesaDto))
                    continue;

                if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
                {
                    Notificar(
                        EnumTipoNotificacao.NotFount,
                        string.Format(
                            Message.IdNaoEncontrado,
                            "A categoria",
                            despesaDto.CategoriaId
                        )
                    );
                    continue;
                }

                var despesa = _mapper.Map<Despesa>(despesaDto);
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);

                despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow);
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Nunhuma das despesa é valida para inserir."
                );
                return null;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            if (totalRecebido > despesasParaInserir.Count)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. "
                        + $"total de {totalRecebido - despesasParaInserir.Count} invalidas."
                );
            }

            var ids = despesasParaInserir.Select(d => d.Id).ToList();
            var despesasInseridas = await _repository
                .Get(d => ids.Contains(d.Id))
                .Include(c => c.Categoria)
                .Include(c => c.GrupoDespesa)
                .ToListAsync();

            return despesasInseridas;
        }

        public async Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            var despesa = await _repository.GetByIdAsync(id);
            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", id)
                );
                return null;
            }

            if (!await ValidarDespesaAsync(despesaDto, id))
                return null;

            _mapper.Map(despesaDto, despesa);

            despesa.Total = despesa.Preco * despesa.Quantidade;
            despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow);

            _repository.Update(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var despesa = await _repository.GetByIdAsync(id);

            if (despesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A despesa", id)
                );
                return false;
            }

            _repository.Delete(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );
                return false;
            }

            return true;
        }
        #endregion


        #region Metodos de Suporte

        private async Task<bool> ValidarDespesaAsync(
            DespesaDto despesaDto,
            int idDespesaInEdicao = 0
        )
        {
            if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "A categoria", despesaDto.CategoriaId)
                );
                return false;
            }

            if (
                despesaDto.CategoriaId == _categoriaIds.IdAluguel
                && !despesaDto.Item.ToLower().Contains("caixa")
                && !despesaDto.Item.ToLower().Contains("parcela ap ponto")
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroAluguelIncorreto);
                return false;
            }

            if (
                despesaDto.CategoriaId == _categoriaIds.IdCondominio
                && !despesaDto.Item.Contains(
                    "condomínio ap ponto",
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.CadastroCondominioIncorreto);
                return false;
            }

            if (await _grupoDespesaRepository.ExisteAsync(despesaDto.GrupoDespesaId) is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(
                        Message.IdNaoEncontrado,
                        "O Grupo de Despesa",
                        despesaDto.GrupoDespesaId
                    )
                );
                return false;
            }

            if (!await IsDespesaExistenteAsync(despesaDto, idDespesaInEdicao))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> IsDespesaExistenteAsync(
            DespesaDto despesaDto,
            int idDespesaInEdicao
        )
        {
            if (!ehDespesaMensal(despesaDto.CategoriaId))
            {
                return true;
            }

            if (despesaDto.Quantidade != 1)
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.DespesaMensalQuantidadeDeveSerUm);
                return false;
            }

            var despesasExistentes = _repository.Get(d =>
                d.GrupoDespesaId == despesaDto.GrupoDespesaId
                && d.CategoriaId == despesaDto.CategoriaId
            );

            if (idDespesaInEdicao != 0)
            {
                despesasExistentes = despesasExistentes.Where(despesa =>
                    despesa.Id != idDespesaInEdicao
                );
            }

            var listExistentes = await despesasExistentes.ToListAsync();
            foreach (var despesa in listExistentes)
            {
                if (
                    despesa.CategoriaId == _categoriaIds.IdAluguel
                    && despesa.Item.Equals(despesaDto.Item, StringComparison.OrdinalIgnoreCase)
                )
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(
                            Message.DespesaExistente,
                            $"{despesa.Categoria.Descricao} {despesa.Item}"
                        )
                    );
                    return false;
                }
                else if (despesa.CategoriaId != _categoriaIds.IdAluguel)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(Message.DespesaExistente, despesa.Categoria.Descricao)
                    );

                    return false;
                }

                return true;
            }

            return true;
        }

        private bool ehDespesaMensal(int idCategoria)
        {
            return idCategoria == _categoriaIds.IdAluguel
                || idCategoria == _categoriaIds.IdCondominio
                || idCategoria == _categoriaIds.IdContaDeLuz
                || idCategoria == _categoriaIds.IdInternet;
        }
        #endregion
    }
}
