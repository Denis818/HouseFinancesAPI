﻿using Application.Constants;
using Application.Interfaces.Services;
using Application.Services.Base;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Dtos.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Finance
{
    public class CategoriaServices(IServiceProvider Service) :
        ServiceAppBase<Categoria, CategoriaDto, ICategoriaRepository>(Service), ICategoriaServices
    {
        public async Task<IEnumerable<Categoria>> GetAllAsync()
            => await _repository.Get().ToListAsync();

        public async Task<Categoria> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<bool> Existe(int id)
            => await _repository.Get().AnyAsync(c => c.Id == id);

        public async Task<Categoria> InsertAsync(CategoriaDto categoriaDto)
        {
            if (Validator(categoriaDto)) return null;

            var categoria = MapToModel(categoriaDto);
            await _repository.InsertAsync(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return categoria;
        }

        public async Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto)
        {
            if (Validator(categoriaDto)) return null;

            var categoria = await _repository.GetByIdAsync(id);

            if (categoria is null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            MapDtoToModel(categoriaDto, categoria);

            _repository.Update(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return categoria;
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(id);

            if (categoria == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return;
            }

            _repository.Delete(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.DeleteError);
                return;
            }

            Notificar(EnumTipoNotificacao.Informacao, "Registro Deletado");
        }

    }
}