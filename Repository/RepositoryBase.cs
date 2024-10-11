using Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext context { get; set; }

        public RepositoryBase(RepositoryContext context)
        {
            this.context = context;
        }

        public void Create(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll(bool trackChanges) => trackChanges ? context.Set<T>() : context.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges) => trackChanges ? 
            context.Set<T>().Where(condition) : context.Set<T>().Where(condition).AsNoTracking();

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
    }
}
