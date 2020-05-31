using FamilyTreeProject.Core.Contracts;
using FamilyTreeProject.Data.Common;

namespace FamilyTreeProject.Data.GEDCOM
{
    public class GEDCOMUnitOfWork : FileUnitOfWork
    {
        public GEDCOMUnitOfWork(string path)
        {
            Requires.NotNullOrEmpty("path", path);
            
            Initialize(new GEDCOMFileStore(path));
        }

        public GEDCOMUnitOfWork(IFileStore store) : base(store) {}
    }
}
