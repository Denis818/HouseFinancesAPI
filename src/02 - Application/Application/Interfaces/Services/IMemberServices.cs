using Domain.Dtos.Finance;
using Domain.Models;
using FamilyFinanceApi.Utilities;

namespace Application.Interfaces.Services
{
    public interface IMemberServices
    {
        Task DeleteAsync(int id);
        IQueryable<Member> GetAllMembersAsync();
        Task<Member> GetByIdAsync(int id);
        Task<Member> InsertAsync(MemberDto memberDto);
        Task<Member> UpdateAsync(int id, MemberDto memberDto);
    }
}