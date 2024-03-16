using Microsoft.EntityFrameworkCore;

namespace Api.Vendas.Utilities
{
    public class PagedResult<T>
    {
        public int TotalItens { get; set; }
        public int PaginaAtual { get; set; }
        public List<T> Itens { get; set; }
    } 

    public class Pagination
    {
        public static async Task<PagedResult<T>> PaginateResult<T>(IQueryable<T> consulta, int numeroPagina, int tamanhoPagina)
            where T : class
        {
            var totalItens = await consulta.CountAsync();
            var quantidadePular = (numeroPagina - 1) * tamanhoPagina;
            var itens = await consulta.Skip(quantidadePular).Take(tamanhoPagina).ToListAsync();

            return new PagedResult<T>
            {
                TotalItens = totalItens,
                PaginaAtual = numeroPagina,
                Itens = itens,
            };
        }
    }
}
