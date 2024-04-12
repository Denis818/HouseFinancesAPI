using Domain.Dtos.Categoria;
using Domain.Models.Finance;

namespace Application.Interfaces.Services.Finance
{
    public interface ICategoriaAppServices
    {
        Task DeleteAsync(int id);
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> InsertAsync(CategoriaDto categoriaDto);
        Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto);
        Task<Categoria> GetByIdAsync(int id);
    }
}
