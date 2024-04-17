using Data.DataContext;
using Data.Repository.Base;
using Domain.Dtos.User;
using Domain.Interfaces.Repositories;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.User
{
    public class UsuarioRepository(IServiceProvider service)
        : RepositoryBase<Usuario, FinanceDbContext>(service),
            IUsuarioRepository
    {
        public override async Task InsertAsync(Usuario usuario)
        {
            await base.InsertAsync(usuario);
            await SaveChangesAsync();
        }

        public async Task AddPermissaoAsync(AddUserPermissionDto userPermissao)
        {
            var usuario = await Get(user => user.Id == userPermissao.UsuarioId)
                .Include(p => p.Permissoes)
                .FirstOrDefaultAsync();

            foreach(var permissao in userPermissao.Permissoes)
            {
                var possuiPermissao = usuario
                    .Permissoes.Where(p => p.Descricao == permissao.ToString())
                    .FirstOrDefault();

                if(possuiPermissao is null)
                {
                    usuario.Permissoes.Add(new Permissao { Descricao = permissao.ToString() });
                }
            }

            Update(usuario);
            await SaveChangesAsync();
        }
    }
}
