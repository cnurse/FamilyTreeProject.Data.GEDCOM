using System;
using System.Collections.Generic;
using System.Linq;
using FamilyTreeProject.Common.Data;
using Naif.Core.Collections;

namespace FamilyTreeProject.Data.GEDCOM
{
    public abstract class BaseRepository<TModel> : IRepository<TModel> where TModel : class
    {
        public bool SupportsAggregates => true;

        public abstract void Add(TModel item);

        public abstract void Delete(TModel item);

        public IEnumerable<TModel> Find(Func<TModel, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public IPagedList<TModel> Find(int pageIndex, int pageSize, Func<TModel, bool> predicate)
        {
            return GetAll().Where(predicate).InPagesOf(pageSize).GetPage(pageIndex);
        }

        IEnumerable<TModel> IRepository<TModel>.GetAll()
        {
            return GetAll();
        }

        public abstract IEnumerable<TModel> GetAll();

        public IPagedList<TModel> GetPage(int pageIndex, int pageSize)
        {
            return GetAll().InPagesOf(pageSize).GetPage(pageIndex);
        }

        public abstract void Update(TModel item);
    }
}