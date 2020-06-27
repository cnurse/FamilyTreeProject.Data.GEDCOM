using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class FamilyRepository : BaseRepository<Family>
    {
        private readonly IGEDCOMFileStore _store;

        public FamilyRepository(IGEDCOMFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }

        public override void Add(Family item)
        {
            Requires.NotNull(item);

            _store.AddFamily(item);
        }

        public override void Delete(Family item)
        {
            Requires.NotNull(item);

            _store.DeleteFamily(item);
        }

        public override IEnumerable<Family> GetAll()
        {
            return _store.Families;
        }

        public override void Update(Family item)
        {
            Requires.NotNull(item);

            _store.UpdateFamily(item);
        }
    }
}