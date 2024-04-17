using Domain.Dtos.Membro;
using Domain.Models.Finance;

namespace Application.Interfaces.Services.Finance
{
    public interface IMembroAppServices
    {
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Membro>> GetAllAsync();
        Task<Membro> GetByIdAsync(int id);
        Task<Membro> InsertAsync(MembroDto memberDto);
        Task<Membro> UpdateAsync(int id, MembroDto memberDto);
    }
}
