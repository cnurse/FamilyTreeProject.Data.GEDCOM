using System;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Common;
using FamilyTreeProject.Core.Contracts;
using FamilyTreeProject.Core.Data;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class GEDCOMUnitOfWork : IUnitOfWork
    {
        private IGEDCOMFileStore _store;
        
        public GEDCOMUnitOfWork(string path)
        {
            Requires.NotNullOrEmpty("path", path);
            
            _store = new GEDCOMFileStore(path);
        }

        public GEDCOMUnitOfWork(IGEDCOMFileStore store)
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
