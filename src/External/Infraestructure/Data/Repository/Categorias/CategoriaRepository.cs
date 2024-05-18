﻿using Data.DataContext;
using Domain.Dtos.Categorias.Consultas;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Infraestructure.Data.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.Categorias
{
    public class CategoriaRepository(IServiceProvider service)
        : RepositoryBase<Categoria, FinanceDbContext>(service),
            ICategoriaRepository
    {
        public async Task<Categoria> ExisteAsync(int id, string nome)
        {
            if (nome != null)
            {
                return await Get(c => c.Descricao == nome).FirstOrDefaultAsync();
            }
            else
            {
                return await Get(c => c.Id == id).FirstOrDefaultAsync();
            }
        }

        public bool IdentificarCategoriaParaAcao(int idCategoria)
        {
            var categoriaIds = GetCategoriaIds();

            var ehAlteravel =
                idCategoria == categoriaIds.IdAluguel
                || idCategoria == categoriaIds.IdCondominio
                || idCategoria == categoriaIds.IdContaDeLuz
                || idCategoria == categoriaIds.IdAlmoco
                || idCategoria == categoriaIds.IdInternet;

            return ehAlteravel;
        }

        public CategoriaIdsDto GetCategoriaIds()
        {
            var categ = Get();

            int idAlmoco = categ.FirstOrDefault(c => c.Descricao == "Almoço/Janta").Id;
            int idAluguel = categ.FirstOrDefault(c => c.Descricao == "Aluguel").Id;
            int idCondominio = categ.FirstOrDefault(c => c.Descricao == "Condomínio").Id;
            int idContaDeLuz = categ.FirstOrDefault(c => c.Descricao == "Conta de Luz").Id;
            int idInternet = categ.FirstOrDefault(c => c.Descricao == "Internet").Id;

            return new CategoriaIdsDto
            {
                IdAluguel = idAluguel,
                IdCondominio = idCondominio,
                IdContaDeLuz = idContaDeLuz,
                IdAlmoco = idAlmoco,
                IdInternet = idInternet
            };
        }
    }
}
