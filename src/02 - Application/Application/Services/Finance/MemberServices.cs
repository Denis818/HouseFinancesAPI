﻿using Application.Constants;
using Application.Services.Base;
using Application.Services.Finance;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using FamilyFinanceApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Services
{
    public class MemberServices(IServiceProvider service) :
        ServiceAppBase<Member, MemberDto, IMemberRepository>(service), IMemberServices
    {
        public IQueryable<Member> GetAllAsync() => _repository.Get();

        public async Task<Member> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Member> InsertAsync(MemberDto memberDto)
        {
            if (Validator(memberDto)) return null;

            if (_repository.Get().Any(m => m.Nome == memberDto.Nome)) 
                Notificar(EnumTipoNotificacao.ClientError, $"O membro {memberDto.Nome} já existe.");

            var member = MapToModel(memberDto);

            await _repository.InsertAsync(member);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return member;
        }

        public async Task<Member> UpdateAsync(int id, MemberDto memberDto)
        {
            if (Validator(memberDto)) return null;

            var member = await _repository.GetByIdAsync(id);

            if (member == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            MapDtoToModel(memberDto, member);

            _repository.Update(member);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return member;
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _repository.GetByIdAsync(id);

            if (member == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return;
            }

            _repository.Delete(member);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.DeleteError);
                return;
            }

            Notificar(EnumTipoNotificacao.Informacao, "Registro Deletado");
        }

    }
}
