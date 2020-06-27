using System.Collections.Generic;
using FamilyTreeProject.Core;

namespace FamilyTreeProject.Data.GEDCOM
{
    public interface IGEDCOMFileStore
    {
        Tree Tree { get; }
        
        IList<Family> Families { get; }

        IList<Individual> Individuals { get; }
        
        IList<Repository> Repositories { get; }

        IList<Source> Sources { get; }

        void AddFamily(Family family);

        void AddIndividual(Individual individual);

        void AddRepository(Repository repository);

        void AddSource(Source source);

        void AddTree(Tree tree);

        void DeleteFamily(Family family);

        void DeleteIndividual(Individual individual);

        void DeleteRepository(Repository repository);

        void DeleteSource(Source source);

        void DeleteTree(Tree tree);

        void SaveChanges();

        void UpdateFamily(Family family);

        void UpdateIndividual(Individual individual);

        void UpdateRepository(Repository repository);
        
        void UpdateSource(Source source);

        void UpdateTree(Tree tree);

    }
}