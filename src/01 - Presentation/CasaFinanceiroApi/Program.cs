using CasaFinanceiroApi.Extensios.Application;
using CasaFinanceiroApi.Extensios.Swagger;
using CasaFinanceiroApi.Middleware;
using Data.Configurations;
using DIContainer.DataBaseConfiguration;
using DIContainer.DependencyManagers;

var builder = WebApplication.CreateBuilder(args);

builder.SetupApplication();
CarregarAppSettings(builder.Services, builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerUI();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.UseRouting();

var companyConnections = app.Services.GetRequiredService<CompanyConnectionStrings>();

app.Services.ConfigurarBancoDeDados(companyConnections);

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


void CarregarAppSettings(IServiceCollection services, IConfiguration configuration)
{
    var appSettingsSection = configuration.GetSection(nameof(CompanyConnectionStrings));
    var appSettings = appSettingsSection.Get<CompanyConnectionStrings>();
    services.AddSingleton(appSettings);
}
