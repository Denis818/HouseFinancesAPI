using Application.Configurations.Extensions.Help;
using Application.Constants;
using Application.Interfaces.Services;
using Application.Services.Base;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Dtos.Finance;
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

            (int idAlmoco, int idAluguel) = await GetIdsAluguelAlmocoAsync();

            if (categoria.Id == idAlmoco || categoria.Id == idAluguel)
            {
                Notificar(EnumTipoNotificacao.Informacao,
                    "Essa categoria faz parta da regra de negócio. Não pode ser alterada.");
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

            (int idAlmoco, int idAluguel) = await GetIdsAluguelAlmocoAsync();
            if (categoria.Id == idAlmoco || categoria.Id == idAluguel)
            {
                Notificar(EnumTipoNotificacao.Informacao, 
                    "Essa categoria faz parta da regra de negócio. Não pode ser deletada");
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

        public async Task<(int, int)> GetIdsAluguelAlmocoAsync()
        {
            var categorias = await GetAllAsync();
            var idAlmoco = categorias.FirstOrDefault(c => c.Descricao.StartsWith("Almoço"));
            var idAluguel = categorias.FirstOrDefault(c => c.Descricao.StartsWith("Aluguel"));

            return (idAlmoco.Id, idAluguel.Id);
        }
    }
}
