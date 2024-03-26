﻿using Application.Interfaces.Services;
using Domain.Enumeradores;
using Domain.Models;
using Domain.Models.Dtos.Finance;
using FamilyFinanceApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
   // [AutorizationFinance]
    [Route("api/[controller]")]
    public class CategoriaController(IServiceProvider service,
        ICategoriaServices _categoriaServices) :
        BaseApiController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _categoriaServices.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<Categoria> GetById(int id)
           => await _categoriaServices.GetByIdAsync(id);

        [HttpPost]
       // [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Post(CategoriaDto vendaDto)
            => await _categoriaServices.InsertAsync(vendaDto);

        [HttpPatch]
     //   [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Put(int id, CategoriaDto vendaDto)
            => await _categoriaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
       // [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await _categoriaServices.DeleteAsync(id);

    }
}
