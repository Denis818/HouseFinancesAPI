﻿using Application.Interfaces.Services.Despesas;
using Application.Interfaces.Services.Membros;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Membros;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Application.Services.Membros
{
    public class MembroAppServices(
        IServiceProvider service,
        IDespesaConsultaAppService _despesaConsultaApp
    ) : BaseAppService<Membro, IMembroRepository>(service), IMembroAppServices
    {
        #region CRUD
        public async Task<IEnumerable<Membro>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Nome).ToListAsync();

        public async Task<Membro> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Membro> InsertAsync(MembroDto membroDto)
        {
            if(Validator(membroDto))
                return null;

            membroDto.Nome = membroDto.Nome.Trim();
            membroDto.Telefone = FormatFone(membroDto.Telefone);

            if(await _repository.ExisteAsync(membroDto.Nome) != null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                );
                return null;
            }

            var membro = _mapper.Map<Membro>(membroDto);

            await _repository.InsertAsync(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return membro;
        }

        public async Task<Membro> UpdateAsync(int id, MembroDto membroDto)
        {
            membroDto.Nome = membroDto.Nome.Trim();

            if(Validator(membroDto))
                return null;

            var membro = await _repository.GetByIdAsync(id);

            if(membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return null;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return null;
            }

            if(await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if(membro.Id != membroExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                    );
                    return null;
                }
            }

            _mapper.Map(membroDto, membro);

            _repository.Update(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return membro;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var membro = await _repository.GetByIdAsync(id);

            if(membro == null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return false;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return false;
            }

            _repository.Delete(membro);

            if(!await _repository.SaveChangesAsync())
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

        public async Task<string> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string titleMessage,
            bool isMoradia,
            string pix
        )
        {
            var membro = await _repository.Get(membro => membro.Nome == nome).FirstOrDefaultAsync();

            if(membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.AcaoNaoInvalida, "Membro não encontrado")
                );

                return null;
            }

            if(isMoradia && membro.Nome.Contains("Jhon"))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.AcaoNaoInvalida, "o Jhon não paga aluguel")
                );

                return null;
            }

            string message = isMoradia
                ? await MensagemValoresMoradiaDividosAsync(pix, membro.Nome, titleMessage)
                : await MensagemValoresCasaDividosAsync(pix, membro.Nome, titleMessage);

            string encodedMessage = Uri.EscapeDataString(message);

            membro.Telefone = Regex.Replace(membro.Telefone, "[^0-9]", "");
            string whatsappUrl = $"https://wa.me/{membro.Telefone}?text={encodedMessage}";

            return whatsappUrl;
        }

        #region Metodos de Suporte

        public async Task<string> MensagemValoresCasaDividosAsync(
            string pix,
            string membroNome,
            string titleMessage
        )
        {
            var resumoMensal = await _despesaConsultaApp.GetDespesasDivididasMensalAsync();

            double valorPorMembro =
                resumoMensal
                    .DespesasPorMembro.Where(membro => membro.Nome == membroNome)
                    .FirstOrDefault()
                    ?.ValorDespesaCasa ?? 0;

            string title = titleMessage.IsNullOrEmpty()
                ? $"Olá {membroNome}, tudo bem? Essas são as despesas desse mês:\r\n\r\n"
                : titleMessage + "\r\n\r\n";

            string messageBody =
                $"As despesas de casa vieram com um valor total de: *R${resumoMensal.RelatorioGastosDoMes.TotalGastosCasa:F2}*.\r\n\r\n"
                + $"Dividido para cada vai ficar: *R$ {valorPorMembro:F2}*."
                + $"\r\n\r\nMeu pix: *{pix}*.";

            return title + messageBody;
        }

        public async Task<string> MensagemValoresMoradiaDividosAsync(
            string pix,
            string membroNome,
            string titleMessage
        )
        {
            var resumoMensal = await _despesaConsultaApp.GetDespesasDivididasMensalAsync();

            double valorPorMembro =
                resumoMensal
                    .DespesasPorMembro.Where(membro => membro.Nome == membroNome)
                    .FirstOrDefault()
                    ?.ValorDespesaMoradia ?? 0;

            string title = titleMessage.IsNullOrEmpty()
                ? $"Olá {membroNome}, tudo bem? Essas são as despesas desse mês:\r\n\r\n"
                : titleMessage + "\r\n\r\n";

            string messageBody =
                $"As despesas Moradia (Aluguel, comdomínio, conta de luz) vieram com um valor total de: *R${resumoMensal.RelatorioGastosDoMes.TotalGastosMoradia:F2}*.\r\n\r\n"
                + $"Dividido para cada vai ficar: *R$ {valorPorMembro:F2}*."
                + $"\r\n\r\nMeu pix: *{pix}*.";

            return title + messageBody;
        }

        public static string FormatFone(string telefone)
        {
            string numeros = Regex.Replace(telefone, "[^0-9]", "");

            if(numeros.Length == 10)
            {
                return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 4)}-{numeros.Substring(6)}";
            }
            else if(numeros.Length == 11)
            {
                return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 5)}-{numeros.Substring(7)}";
            }
            else
            {
                return telefone;
            }
        }
        #endregion
    }
}
