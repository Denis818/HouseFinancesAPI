using Domain.Dtos.Finance;
using Domain.Models;
using FamilyFinanceApi.Utilities;

namespace Application.Interfaces.Services.Finance
{
    public interface IMembroServices
    {
        Task DeleteAsync(int id);
        IQueryable<Membro> GetAllAsync();
        Task<Membro> GetByIdAsync(int id);
        Task<Membro> InsertAsync(MembroDto memberDto);
        Task<Membro> UpdateAsync(int id, MembroDto memberDto);
    }
}