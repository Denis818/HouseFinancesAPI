﻿using Domain.Dtos.Membro;
using Domain.Models.Finance;
using HouseFinancesAPI.Utilities;

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