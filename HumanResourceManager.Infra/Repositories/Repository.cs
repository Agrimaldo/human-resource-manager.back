
using HumanResourceManager.Domain.Interfaces;
using HumanResourceManager.Infra.Context;
using System.Linq.Expressions;

namespace HumanResourceManager.Infra.Repositories
{
    public class Repository : IDisposable, IRepository
    {
        private readonly PostgresqlContext _postgresqlContext;
        public Repository(PostgresqlContext postgresqlContext)
        {
            _postgresqlContext = postgresqlContext;
        }

        public T? Add<T>(T obj) where T : class
        {
            _postgresqlContext.Database.BeginTransaction();
            try
            {
                _postgresqlContext.Set<T>().Add(obj);
            }
            catch (Exception)
            {
                _postgresqlContext.Database.RollbackTransaction();
                return null;
            }

            _postgresqlContext.Database.CommitTransaction();
            _postgresqlContext.SaveChanges();
            return obj;
        }

        public bool Delete<T>(T obj) where T : class
        {
            _postgresqlContext.Database.BeginTransaction();
            try
            {
                _postgresqlContext.Set<T>().Remove(obj);
            }
            catch (Exception)
            {
                _postgresqlContext.Database.RollbackTransaction();
                return false;
            }

            _postgresqlContext.Database.CommitTransaction();
            _postgresqlContext.SaveChanges();
            return true;
        }

        public T? Update<T>(T obj) where T : class
        {
            _postgresqlContext.Database.BeginTransaction();
            try
            {
                _postgresqlContext.Set<T>().Update(obj);
            }
            catch (Exception)
            {
                _postgresqlContext.Database.RollbackTransaction();
                return null;
            }

            _postgresqlContext.Database.CommitTransaction();
            _postgresqlContext.SaveChanges();
            return obj;
        }

        public bool Exists<T>(Expression<Func<T, bool>>? conditional = null) where T : class
        {
            if (conditional == null)
                return false;

            return _postgresqlContext.Set<T>().Where(conditional).Any();
        }

        public int Count<T>(Expression<Func<T, bool>>? conditional = null) where T : class
        {
            if (conditional == null)
                conditional = p => true;

            return _postgresqlContext.Set<T>().Where(conditional).Count();
        }

        public List<T> List<T>(int skip = 0, int take = 50, Expression<Func<T, bool>>? conditional = null) where T : class
        {
            if (conditional == null)
                conditional = p => true;

            return _postgresqlContext.Set<T>().Where(conditional).Skip(skip).Take(take).ToList();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _postgresqlContext.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
