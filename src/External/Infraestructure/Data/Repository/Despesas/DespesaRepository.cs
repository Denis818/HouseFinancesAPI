using Data.DataContext;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Infraestructure.Data.Configurations;
using Infraestructure.Data.Repository.Base;
using Microsoft.AspNetCore.Http;
using MySqlConnector;

namespace Infraestructure.Data.Repository.Despesas
{
    public class DespesaRepository : RepositoryBase<Despesa, FinanceDbContext>, IDespesaRepository
    {
        private readonly CompanyConnectionStrings _companyConnections;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DespesaRepository(
            IServiceProvider service,
            CompanyConnectionStrings companyConnections,
            IHttpContextAccessor httpContextAccessor
        )
            : base(service)
        {
            _companyConnections = companyConnections;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Despesa>> GetDespesasUsingMySqlConnector(int grupoDespesaId)
        {
            var despesas = new List<Despesa>();

            var query =
                @"
            SELECT d.Id, d.DataCompra, d.Item, d.Preco, d.Quantidade, d.Fornecedor, d.Total, 
                   d.GrupoDespesaId, d.CategoriaId, 
                   g.Id AS GrupoDespesaId, g.Nome AS GrupoDespesaNome,
                   c.Id AS CategoriaId, c.Descricao AS CategoriaDescricao
            FROM Despesas d
            INNER JOIN Grupo_Despesa g ON d.GrupoDespesaId = g.Id
            INNER JOIN Categorias c ON d.CategoriaId = c.Id
            WHERE d.GrupoDespesaId = @GrupoDespesaId";

            var connectionString = IdentificarStringConexao(_httpContextAccessor.HttpContext);

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GrupoDespesaId", grupoDespesaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var despesa = new Despesa
                            {
                                Id = reader.GetInt32("Id"),
                                DataCompra = reader.GetDateTime("DataCompra"),
                                Item = reader.GetString("Item"),
                                Preco = reader.GetDouble("Preco"),
                                Quantidade = reader.GetInt32("Quantidade"),
                                Fornecedor = reader.GetString("Fornecedor"),
                                Total = reader.GetDouble("Total"),
                                GrupoDespesaId = reader.GetInt32("GrupoDespesaId"),
                                CategoriaId = reader.GetInt32("CategoriaId"),
                                GrupoDespesa = new GrupoDespesa
                                {
                                    Id = reader.GetInt32("GrupoDespesaId"),
                                    Nome = reader.GetString("GrupoDespesaNome")
                                },
                                Categoria = new Categoria
                                {
                                    Id = reader.GetInt32("CategoriaId"),
                                    Descricao = reader.GetString("CategoriaDescricao")
                                }
                            };

                            despesas.Add(despesa);
                        }
                    }
                }
            }

            return despesas;
        }

        private string IdentificarStringConexao(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"].ToString();

            if (string.IsNullOrEmpty(origin))
            {
                origin = $"https://{context.Request.Host}";
            }

            var originUri = new Uri(origin);
            var hostName = originUri.Host;

            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio == hostName
            );

            if (empresaLocalizada == null)
            {
                throw new Exception($"A empresa com nome de domínio '{hostName}' não existe");
            }

            return empresaLocalizada.ConnectionString;
        }
    }
}
