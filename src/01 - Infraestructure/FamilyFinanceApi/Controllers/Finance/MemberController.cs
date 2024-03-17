﻿using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Models.Finance;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [PermissoesFinance]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MemberController(IServiceProvider service, 
        IMemberServices MemberServices) : 
        BaseApiController(service)
    {
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PageMemberExample))]
        public async Task<PagedResult<Member>> GetAllDespesaAsync(int paginaAtual = 1, int itensPorPagina = 10) =>
            await MemberServices.GetAllMembersAsync(paginaAtual, itensPorPagina);

        [HttpGet("{id}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> GetById(int id)
           => await MemberServices.GetByIdAsync(id);

        [HttpPost]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> Post(MemberDto vendaDto)
            => await MemberServices.InsertAsync(vendaDto);

        [HttpPatch]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> Put(int id, MemberDto vendaDto)
            => await MemberServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        public async Task Delete(int id)
            => await MemberServices.DeleteAsync(id);

    }
}
