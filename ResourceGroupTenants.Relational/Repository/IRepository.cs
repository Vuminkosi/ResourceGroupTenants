using ResourceGroupTenants.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Repository
{
    public interface IRepository<T> where T : BaseModel
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(string resourceCode, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(string Id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> orderBy, CancellationToken cancellationToken = default);

        Task SaveAsync(T model, CancellationToken cancellationToken = default);
        Task<T> AddOrUpdateAsync(T model, bool isUpdate = default, CancellationToken cancellationToken = default);
        Task SaveAllAsync(IEnumerable<T> models, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> models, bool isUpdate = default, CancellationToken cancellationToken = default);
        Task DeleteAsync(string primaryKey, CancellationToken cancellationToken = default);

        Task DeleteAllAsync(IEnumerable<string> primaryKey, CancellationToken cancellationToken = default);

        Task<bool> HasAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    }


}
