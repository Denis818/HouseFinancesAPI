﻿using Application.Utilities;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissoesFinance(params EnumPermissoes[] enumPermissoes) : Attribute, IAuthorizationFilter
    {
        private IEnumerable<string> EnumPermissoes { get; } = enumPermissoes.Select(x => x.ToString());

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var possuiTodasPermissoes = EnumPermissoes.All(permissao =>
            //context.HttpContext.User.Claims.Any(claim => claim.Value == permissao));

            //if (!possuiTodasPermissoes)
            //{
            //    var response = new ResponseResultDTO<string>()
            //    {
            //        Mensagens = [new Notificacao("Você não tem permissão para acessar esse recurso.")]
            //    };

            //    context.Result = new ObjectResult(response) { StatusCode = 401 };
            //    return;
            //}
        }
    }
}
