using Domain.Dtos.Membros;
using Domain.Models.Membros;

namespace Application.Interfaces.Services.Membros
{
    public interface IMembroAppServices
    {
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Membro>> GetAllAsync();
        Task<Membro> GetByIdAsync(int id);
        Task<Membro> InsertAsync(MembroDto memberDto);
        Task<Membro> UpdateAsync(int id, MembroDto memberDto);
        Task<string> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string titleMessage,
            bool isHabitacional,
            string pix
        );
    }
}
