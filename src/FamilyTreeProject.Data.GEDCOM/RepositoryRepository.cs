using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class RepositoryRepository: BaseRepository<Repository>
    {
        private readonly IGEDCOMFileStore _store;

        public RepositoryRepository(IGEDCOMFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }

        public override void Add(Repository item)
        {
            Requires.NotNull(item);

            _store.AddRepository(item);
        }

        public override void Delete(Repository item)
        {
            Requires.NotNull(item);

            //_store.DeleteRepository(item);
        }

        public override IEnumerable<Repository> GetAll()
        {
            return _store.Repositories;
        }

        public override void Update(Repository item)
        {
            Requires.NotNull(item);

            //_store.UpdateRepositoryl(item);
        }
    }
}