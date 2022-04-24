using Microsoft.EntityFrameworkCore;

using ResourceGroupTenants.Core.Models;
using ResourceGroupTenants.Relational.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGroupTenants.Relational.Repository
{
    public class AtomicRepository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly DbContext _context;

        public AtomicRepository(DbContext dBContext)
        {
            this._context = dBContext;
        }



        public virtual async Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> models, bool isUpdate = default, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(models);
            foreach (var model in models)
                await AddOrUpdateAsync(model, isUpdate, cancellationToken);

            return models;
        }

        public virtual async Task<T> AddOrUpdateAsync(T model, bool isUpdate = false, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(model);

            if (!isUpdate)
            {
                var updateValue = await GetByIdAsync(model.Id, cancellationToken);

                if (updateValue == null)
                {
                    _context.Set<T>().Add(model);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                    await this.AddOrUpdateAsync(model, true, cancellationToken);


            }
            else
            {

                var updateValue = await GetByIdAsync(model.Id, cancellationToken);

                if (updateValue != null)
                {

                    //update to local collection
                    _context.Set<T>().Remove(updateValue);

                    //Commit to database
                    await _context.SaveChangesAsync(cancellationToken);

                    // Add New Record
                    await this.AddOrUpdateAsync(model, false, cancellationToken);
                }
                else
                {

                    // Add New Record
                    await this.AddOrUpdateAsync(model, false, cancellationToken);
                }
            }

            return model;
        }

        public virtual Task DeleteAllAsync(IEnumerable<string> primaryKey, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(primaryKey);

            ParallelLoopResult response = Parallel.ForEach(primaryKey,
                async (primaryKey) =>
                {
                    await DeleteAsync(primaryKey, cancellationToken);
                });

            return Task.FromResult(response);
        }

        public virtual async Task DeleteAsync(string primaryKey, CancellationToken cancellationToken = default)
        {
            ParameterIsNullOrWhiteSpaceCheck(primaryKey, nameof(primaryKey));
            var model = await GetByIdAsync(primaryKey, cancellationToken);
            if (model is not null)
            {
                _context.Set<T>().Remove(model);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


        public async virtual Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AsNoTracking()
                  .Where(model => !model.IsMarkedForDelete)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(string resourceCode, CancellationToken cancellationToken = default)
        {
            ParameterIsNullOrWhiteSpaceCheck(resourceCode, nameof(resourceCode));
            return await _context.Set<T>().AsNoTracking()
                .Where(model => model.ResourceCode == resourceCode && !model.IsMarkedForDelete)
                .ToListAsync(cancellationToken)
                ;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);

            return await _context.Set<T>().AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<string>> GetAllIdsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);
            return await _context.Set<T>().AsNoTracking()
                .Where(predicate)
                .OrderByDescending(x => x.UpdateDate)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken)
                ;

        }

        public virtual async Task<IEnumerable<T>> GetAllAsync<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> orderBy, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);

            ModelParameterNullCheck(orderBy);

            return await _context.Set<T>().AsNoTracking()
                .Where(predicate)
                .OrderByDescending(orderBy)
                .ToListAsync(cancellationToken)
                ;
        }

        public virtual async Task<T> GetByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return default;

            return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == Id, cancellationToken: cancellationToken);
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);
            return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            ModelParameterNullCheck(predicate);
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public virtual Task SaveAllAsync(IEnumerable<T> models, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(models);

            Parallel.ForEach(models,
                 async (model) =>
                 {
                     await SaveAsync(model, cancellationToken);
                 });

            return Task.CompletedTask;
        }

        public virtual Task SaveAsync(T model, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(model);

            return this.AddOrUpdateAsync(model, true, cancellationToken);
        }

        public virtual async Task<bool> HasAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);
            return await _context.Set<T>().AnyAsync(predicate, cancellationToken: cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ModelParameterNullCheck(predicate);

            return await _context.Set<T>().AsNoTracking()
                   .Where(predicate)
                 .CountAsync(cancellationToken)
                 ;
        }


        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AsNoTracking()
                 .CountAsync(cancellationToken)
                 ;
        }


        public virtual void ParameterIsNullOrWhiteSpaceCheck(string param, string name)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException($"'{name}' cannot be null or whitespace.", name);
            }
        }

        public virtual void ModelParameterNullCheck<TType>(TType predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
        }


    }
}
