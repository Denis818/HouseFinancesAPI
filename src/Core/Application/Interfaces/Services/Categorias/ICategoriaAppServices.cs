using Domain.Dtos.Categorias;
using Domain.Models.Categorias;

namespace Application.Interfaces.Services.Categorias
{
    public interface ICategoriaAppServices
    {
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> InsertAsync(CategoriaDto categoriaDto);
        Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto);
        Task<Categoria> GetByIdAsync(int id);
    }
}
