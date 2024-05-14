using Web.Extensios.Application;
using Web.Extensios.DependencyManagers;
using Web.Extensios.Swagger;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplication();

var app = builder.Build();

app.ConfigureSwaggerUI();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<IdentificadorDataBaseMiddleware>();

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
