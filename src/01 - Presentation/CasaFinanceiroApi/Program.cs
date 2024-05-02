using CasaFinanceiroApi.Extensios.Application;
using CasaFinanceiroApi.Extensios.Swagger;
using CasaFinanceiroApi.Middleware;
using DIContainer.DataBaseConfiguration;
using DIContainer.DependencyManagers;

var builder = WebApplication.CreateBuilder(args);

builder.SetupApplication();
builder.Services.AddProblemDetails();

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
        context.Response.Redirect("/doc");
        await Task.CompletedTask;
    }
);

app.Run();
