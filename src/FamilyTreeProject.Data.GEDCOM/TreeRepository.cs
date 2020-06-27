using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class TreeRepository : BaseRepository<Tree>
    {
        private readonly IGEDCOMFileStore _store;

        public TreeRepository(IGEDCOMFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }

        public override void Add(Tree item)
        {
            Requires.NotNull(item);

            _store.AddTree(item);
        }

        public override void Delete(Tree item)
        {
            Requires.NotNull(item);

            _store.DeleteTree(item);
        }

        public override IEnumerable<Tree> GetAll()
        {
            return new List<Tree> {_store.Tree};
        }

        public override void Update(Tree item)
        {
            Requires.NotNull(item);

            _store.UpdateTree(item);
        }
    }
}