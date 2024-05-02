﻿using Data.DataContext;
using Data.Repository.Base;
using Domain.Interfaces.Repositories;
using Domain.Models.Users;

namespace Data.Repository.User
{
    public class UsuarioRepository(IServiceProvider service)
        : RepositoryBase<Usuario, FinanceDbContext>(service),
            IUsuarioRepository
    {
    }
}
