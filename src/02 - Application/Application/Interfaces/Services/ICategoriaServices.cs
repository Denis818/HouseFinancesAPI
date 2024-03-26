using Domain.Models;
using Domain.Models.Dtos.Finance;

namespace Application.Interfaces.Services
{
    public interface ICategoriaServices
    {
        Task<bool> Existe(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> InsertAsync(CategoriaDto categoriaDto);
        Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto);
        Task<Categoria> GetByIdAsync(int id);
        Task<(int, int)> GetIdsAluguelAlmocoAsync();
    }
}