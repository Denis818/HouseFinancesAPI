using Microsoft.EntityFrameworkCore;

namespace Application.Utilities
{
    public class PagedResult<T>
    {
        public int TotalItens { get; set; }
        public int PaginaAtual { get; set; }
        public List<T> Itens { get; set; }
    }

    public class Pagination
    {
        public static async Task<PagedResult<T>> PaginateResultAsync<T>(
            IQueryable<T> consulta,
            int numeroPagina = 1,
            int tamanhoPagina = 10
        )
            where T : class
        {
            int totalItens = await consulta.CountAsync();
            int quantidadePular = 0;

            if (totalItens > tamanhoPagina)
            {
                quantidadePular = (numeroPagina - 1) * tamanhoPagina;
            }

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
