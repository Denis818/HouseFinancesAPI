using Domain.Dtos.Finance;
using Domain.Models.Finance;
using FamilyFinanceApi.Utilities;

namespace Application.Interfaces.Services
{
    public interface IMemberServices
    {
        Task DeleteAsync(int id);
        Task<PagedResult<Member>> GetAllMembersAsync(int paginaAtual, int itensPorPagina);
        Task<Member> GetByIdAsync(int id);
        Task<Member> InsertAsync(MemberDto memberDto);
        Task<Member> UpdateAsync(int id, MemberDto memberDto);
    }
}