using System;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using Naif.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class GEDCOMUnitOfWork : IUnitOfWork
    {
        private IFileStore _store;
        
        public GEDCOMUnitOfWork(string path)
        {
            Requires.NotNullOrEmpty("path", path);
            
            _store = new GEDCOMFileStore(path);
        }

        public GEDCOMUnitOfWork(IFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }
        
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Commit()
        {
            _store.SaveChanges();
        }

        public IRepository<T> GetRepository<T>() where T : Entity
        {
            if (typeof(T) == typeof(Tree))
            {
                return new TreeRepository(_store) as IRepository<T>;
            }
            if (typeof(T) == typeof(Individual))
            {
                return new IndividualRepository(_store) as IRepository<T>;
            }
            if (typeof(T) == typeof(Family))
            {
                return new FamilyRepository(_store) as IRepository<T>;
            }
            if (typeof(T) == typeof(Repository))
            {
                return new RepositoryRepository(_store) as IRepository<T>;
            }
            if (typeof(T) == typeof(Source))
            {
                return new SourceRepository(_store) as IRepository<T>;
            }
            throw new NotImplementedException();
        }
    }
}
