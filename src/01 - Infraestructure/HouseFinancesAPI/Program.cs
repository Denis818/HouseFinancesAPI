using Application.Configurations.Extensions.DependencyManagers;
using Data.Configurations.Extensions;
using HouseFinancesAPI.Extensios.Application;
using HouseFinancesAPI.Extensios.Swagger;
using ProEventos.API.Configuration.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.SetupApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiDependencyServices(builder.Configuration);
builder.Services.AddSwaggerAuthorizationJWT();

var app = builder.Build();

app.ConfigureSwaggerUI();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.UseRouting();

app.Services.ConfigurarBancoDados();
app.UseMiddleware<MiddlewareException>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapGet(
    "/{*path}",
    async context =>
    {
        context.Response.Redirect("/Doc");
        await Task.CompletedTask;
    }
);

app.Run();
