using Domain.Enumeradores;

namespace Application.Interfaces.Services
{
    public interface IUserServices
    {
        public string Name { get; }
        bool PossuiPermissao(params EnumPermissoes[] permissoesParaValidar);
    }
}
