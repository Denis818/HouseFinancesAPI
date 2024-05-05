﻿using Application.Interfaces.Services.User;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.User;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CasaFinanceiroApi.V1.User
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v1/user")]
    public class UsersController(IAuthAppService _authService, IServiceProvider service)
        : BaseApiController(service)
    {
        [HttpPost("login")]
        public async Task<UserTokenDto> Login(UserDto userDto)
        {
            if(userDto.Email.IsNullOrEmpty() || userDto.Password.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            return await _authService.AutenticarUsuario(userDto);
        }

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() =>
            new(
                HttpContext.User.Identity.Name,
                _authService.VerificarPermissao(EnumPermissoes.USU_000001)
            );
    }
}
