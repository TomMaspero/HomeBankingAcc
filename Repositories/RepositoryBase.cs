using HomeBankingAcc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HomeBankingAcc.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected HomeBankingContext RepositoryContext { get; set; }
        protected RepositoryBase(HomeBankingContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }
        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> queryable = this.RepositoryContext.Set<T>();
            if (includes != null)
            {
                queryable = includes(queryable);
            }
            return queryable.AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().
                Where(expression)
                .AsNoTrackingWithIdentityResolution();
        }

        public void SaveChanges()
        {
            this.RepositoryContext.SaveChanges();
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Update(entity);
        }
    }
}
