using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Data.Common;
using Naif.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class SourceRepository: BaseRepository<Source>
    {
        private readonly IFileStore _store;

        public SourceRepository(IFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }

        public override void Add(Source item)
        {
            Requires.NotNull(item);

            _store.AddSource(item);
        }

        public override void Delete(Source item)
        {
            Requires.NotNull(item);

            //_store.DeleteRepository(item);
        }

        public override IEnumerable<Source> GetAll()
        {
            return _store.Sources;
        }

        public override void Update(Source item)
        {
            Requires.NotNull(item);

            //_store.UpdateRepositoryl(item);
        }
    }
}