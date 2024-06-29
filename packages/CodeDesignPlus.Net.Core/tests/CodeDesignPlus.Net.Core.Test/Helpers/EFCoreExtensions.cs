using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.Core.Test.Helpers
{
    /// <summary>
    /// Clase que contiene metodos de extensión para el objeto IQueryable<T>
    /// </summary>
    public static class EFCoreExtensions
    {
        /// <summary>
        /// Metodo de extensión que se encarga de paginar la información desde la base de datos
        /// </summary>
        /// <typeparam name="TEntity">Entidad a consultar</typeparam>
        /// <param name="query">Consulta de EF Core a ejecutar</param>
        /// <param name="currentPage">Pagina actual</param>
        /// <param name="pageSize">Numero de registros en la pagina</param>
        /// <returns>Represents an asynchronous operation that can return a value.</returns>
        public static async Task<Pager<TEntity>> ToPageAsync<TEntity>(this IQueryable<TEntity> query, int currentPage, int pageSize) where TEntity : IEntity
        {
            if (currentPage < 1 || pageSize < 1)
                return new Pager<TEntity>(0, null!, 0, 0);

            var totalItems = await query.CountAsync();

            var skip = (currentPage - 1) * pageSize;

            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new Pager<TEntity>(totalItems, data, currentPage, pageSize);
        }
    }
}
