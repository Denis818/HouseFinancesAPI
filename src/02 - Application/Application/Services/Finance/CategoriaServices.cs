using Application.Configurations.Extensions.Help;
using Application.Constants;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using Domain.Dtos.Categoria;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text.RegularExpressions;

namespace Application.Services.Finance
{
    public class CategoriaServices(IServiceProvider Service) :
        ServiceAppBase<Categoria, CategoriaDto, ICategoriaRepository>(Service), ICategoriaServices
    {
        #region CRUD
        public async Task<IEnumerable<Categoria>> GetAllAsync()
            => await _repository.Get().ToListAsync();

        public async Task<Categoria> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Categoria> InsertAsync(CategoriaDto categoriaDto)
        {
            if (Validator(categoriaDto)) return null;

            if (await _repository.ExisteAsync(nome:categoriaDto.Descricao) != null)
            {
                Notificar(EnumTipoNotificacao.ClientError, $"Categoria {categoriaDto.Descricao} já existe.");
                return null;
            }

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

            if (_repository.ValidaCategoriaParaAcao(categoria.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError,
                    "Essa categoria faz parta da regra de negócio. Não pode ser alterada.");
                return null;
            }          

            if (await _repository.ExisteAsync(nome: categoriaDto.Descricao) is Categoria catergoriaExiste)
            {
                if (categoria.Id != catergoriaExiste.Id)
                {
                    Notificar(EnumTipoNotificacao.ClientError, $"Categoria {categoriaDto.Descricao} já existe.");
                    return null;
                }
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

            if (_repository.ValidaCategoriaParaAcao(categoria.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError,
                    "Essa categoria faz parta da regra de negócio. Não pode ser alterada");
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
        #endregion
    }
}
