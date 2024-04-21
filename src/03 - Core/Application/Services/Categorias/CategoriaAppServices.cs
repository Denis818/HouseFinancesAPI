using Application.Constants;
using Application.Interfaces.Services.Categorias;
using Application.Services.Base;
using Domain.Dtos.Categorias;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Categorias
{
    public class CategoriaAppServices(IServiceProvider service)
        : BaseAppService<Categoria, ICategoriaRepository>(service),
            ICategoriaAppServices
    {
        #region CRUD
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Descricao).ToListAsync();

        public async Task<Categoria> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Categoria> InsertAsync(CategoriaDto categoriaDto)
        {
            categoriaDto.Descricao = categoriaDto.Descricao.Trim();

            if(Validator(categoriaDto))
                return null;

            if(await _repository.ExisteAsync(nome: categoriaDto.Descricao) != null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    $"Categoria {categoriaDto.Descricao} já existe."
                );
                return null;
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            await _repository.InsertAsync(categoria);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return categoria;
        }

        public async Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto)
        {
            categoriaDto.Descricao = categoriaDto.Descricao.Trim();

            if(Validator(categoriaDto))
                return null;

            var categoria = await _repository.GetByIdAsync(id);

            if(categoria is null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            if(categoria.Descricao == categoriaDto.Descricao)
                return categoria;

            if(_repository.ValidaCategoriaParaAcao(categoria.Id))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Essa categoria faz parta da regra de negócio. Não pode ser alterada."
                );
                return null;
            }

            if(
                await _repository.ExisteAsync(nome: categoriaDto.Descricao)
                is Categoria catergoriaExiste
            )
            {
                if(categoria.Id != catergoriaExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        $"Categoria {categoriaDto.Descricao} já existe."
                    );
                    return null;
                }
            }

            _mapper.Map(categoriaDto, categoria);

            _repository.Update(categoria);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return categoria;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(id);

            if(categoria == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return false;
            }

            if(_repository.ValidaCategoriaParaAcao(categoria.Id))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Essa categoria faz parta da regra de negócio. Não pode ser alterada"
                );
                return false;
            }

            _repository.Delete(categoria);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.DeleteError);
                return false;
            }

            Notificar(EnumTipoNotificacao.Informacao, "Registro Deletado");
            return true;
        }
        #endregion
    }
}
