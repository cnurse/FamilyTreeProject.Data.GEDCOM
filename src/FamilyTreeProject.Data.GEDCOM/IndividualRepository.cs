using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Contracts;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class IndividualRepository : BaseRepository<Individual>
    {
        private readonly IGEDCOMFileStore _store;

        public IndividualRepository(IGEDCOMFileStore store)
        {
            Requires.NotNull(store);

            _store = store;
        }

        public override void Add(Individual item)
        {
            Requires.NotNull(item);

            _store.AddIndividual(item);
        }

        public override void Delete(Individual item)
        {
            Requires.NotNull(item);

            _store.DeleteIndividual(item);
        }

        public override IEnumerable<Individual> GetAll()
        {
            return _store.Individuals;
        }

        public override void Update(Individual item)
        {
            Requires.NotNull(item);

            _store.UpdateIndividual(item);
        }
    }
}