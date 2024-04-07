using Data.DataContext.Context;
using Data.Repository.Base;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Data.Repository.User
{
    public class UsuarioRepository(IServiceProvider service) :
        RepositoryBase<Usuario, FinanceDbContext>(service), IUsuarioRepository
    {

        public override async Task InsertAsync(Usuario usuario)
        {
            await base.InsertAsync(usuario);
            await SaveChangesAsync();
        }
    }
}
