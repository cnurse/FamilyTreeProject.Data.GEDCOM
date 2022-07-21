using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Common;
using FamilyTreeProject.GEDCOM;
using FamilyTreeProject.GEDCOM.Common;
using FamilyTreeProject.GEDCOM.Records;
using FamilyTreeProject.GEDCOM.Structures;
using Naif.Core.Contracts;
using FamilyTreeProject.Common;

// ReSharper disable UseNullPropagation

// ReSharper disable InconsistentNaming
// ReSharper disable UseStringInterpolation

namespace FamilyTreeProject.Data.GEDCOM
{
    public class GEDCOMFileStore : IGEDCOMFileStore
    {
        private readonly string DEFAULT_TREE_ID = Guid.Empty.ToString();
        private readonly string _path;
        private readonly GEDCOMDocument _gedComDocument;

        public GEDCOMFileStore(string path)
        {
            Requires.NotNullOrEmpty("path", path);

            _path = path;
            _gedComDocument = new GEDCOMDocument();
            
            Initialize();
        }

        private void Initialize()
        {
            LoadDocument();

            Tree = new Tree();
            
            Families = new List<Family>();
            Individuals = new List<Individual>();
            Repositories = new List<Repository>();
            Sources = new List<Source>();

            LoadTree();
            LoadIndividuals();
            LoadFamilies();
            LoadRepositories();
            LoadSources();
        }

        private void LoadDocument()
        {
            using (var stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                _gedComDocument.Load(stream);
            }
        }        

        public Tree Tree { get; private set; }
        public IList<Family> Families { get; private set; }
        public IList<Individual> Individuals { get; private set; }
        public IList<Repository> Repositories { get; private set; }
        public IList<Source> Sources { get; private set; }
        
        public void SaveChanges()
        {
            using (var stream = new FileStream(_path, FileMode.Create, FileAccess.Write))
            {
                _gedComDocument.Save(stream);
            }
        }

        private void CreateNewFamily(Individual individual)
        {
            var newFamily = new Family
            {
                HusbandId = individual.FatherId,
                WifeId = individual.MotherId
            };

            newFamily.Children.Add(individual);

            //Save Family
            AddFamily(newFamily);
        }

        private GEDCOMFamilyRecord GetFamilyRecord(Individual individual)
        {
            string fatherId = individual.FatherId;
            string motherId = individual.MotherId;

            var familyRecord = (!string.IsNullOrEmpty(fatherId))
                ? !(string.IsNullOrEmpty(motherId))
                    ? _gedComDocument.SelectFamilyRecord(GEDCOMUtil.CreateId("I", fatherId), GEDCOMUtil.CreateId("I", motherId))
                    : _gedComDocument.SelectHusbandsFamilyRecords(GEDCOMUtil.CreateId("I", fatherId)).FirstOrDefault()
                : _gedComDocument.SelectWifesFamilyRecords(GEDCOMUtil.CreateId("I", motherId)).FirstOrDefault();
            return familyRecord;
        }

        private void ProcessCitations(Entity entity, List<GEDCOMSourceCitationStructure> citations)
        {
            foreach (var citationStructure in citations)
            {
                if (citationStructure == null) continue;

                var newCitation = new Citation()
                                        {
                                            Date = citationStructure.Date,
                                            Page = citationStructure.Page,
                                            Text = citationStructure.Text,
                                            SourceId = citationStructure.XRefId
//                                            OwnerId = entity.Id,
//                                            OwnerType = (entity is Individual) 
//                                                        ? EntityType.Individual
//                                                        :(entity is Fact) 
//                                                            ? EntityType.Fact
//                                                            : EntityType.Family
                                        };

                if (entity is Fact factEntity)
                {
                    factEntity.Citations.Add(newCitation);
                }
                else
                {
                    if (entity is AncestorEntity ancestorEntity)
                    {
                        ancestorEntity.Citations.Add(newCitation);
                    }
                }

                ProcessMultimedia(entity, citationStructure.Multimedia);

                ProcessNotes(entity, citationStructure.Notes);
            }
        }

        private void ProcessFacts(AncestorEntity entity, List<GEDCOMEventStructure> events)
        {
            foreach (var eventStructure in events)
            {
                var newFact = new Fact()
                                    {
                                        Date = eventStructure.Date,
                                        Place = (eventStructure.Place != null) ? eventStructure.Place.Data : string.Empty
//                                        OwnerId = entity.Id,
//                                        OwnerType = (entity is Individual) ? EntityType.Individual : EntityType.Family
                                    };

                switch (eventStructure.EventClass)
                {
                    case EventClass.Individual:
                        newFact.FactType = (FactType) Enum.Parse(typeof(FactType), eventStructure.IndividualEventType.ToString());
                        break;
                    case EventClass.Family:
                        newFact.FactType = (FactType) Enum.Parse(typeof(FactType), eventStructure.FamilyEventType.ToString());
                        break;
                    case EventClass.Attribute:
                        newFact.FactType = (FactType) Enum.Parse(typeof(FactType), eventStructure.IndividualAttributeType.ToString());
                        break;
                    default:
                        newFact.FactType = FactType.Unknown;
                        break;
                }
                entity.Facts.Add(newFact);

                ProcessMultimedia(newFact, eventStructure.Multimedia);

                ProcessNotes(newFact, eventStructure.Notes);

                ProcessCitations(newFact, eventStructure.SourceCitations);
            }
        }

        private  void LoadFamilies()
        {
            foreach (var gedcomRecord in _gedComDocument.FamilyRecords)
            {
                var familyRecord = (GEDCOMFamilyRecord) gedcomRecord;
                var family = new Family
                                    {
                                        // ReSharper disable once PossibleInvalidOperationException
                                        Id = familyRecord.GetId().Value,
                                        HusbandId = familyRecord.Husband,
                                        WifeId = familyRecord.Wife,
                                        TreeId = DEFAULT_TREE_ID
                                    };

                ProcessFacts(family, familyRecord.Events);

                ProcessMultimedia(family, familyRecord.Multimedia);

                ProcessNotes(family, familyRecord.Notes);

                ProcessCitations(family, familyRecord.SourceCitations);

                foreach (string child in familyRecord.Children)
                {
                    var childId = GEDCOMUtil.GetId(child);
                    if (childId > -1)
                    {
                        var individual = Individuals.SingleOrDefault(ind => ind.Id == childId);
                        if (individual != null)
                        {
                            individual.MotherId = family.WifeId;
                            individual.FatherId = family.HusbandId;
                        }
                    }
                }
                Families.Add(family);
            }
        }

        private  void LoadIndividuals()
        {
            foreach (var gedcomRecord in _gedComDocument.IndividualRecords)
            {
                var individualRecord = (GEDCOMIndividualRecord) gedcomRecord;
                var individual = new Individual
                                        {
                                            // ReSharper disable once PossibleInvalidOperationException
                                            Id = individualRecord.GetId().Value,
                                            FirstName = (individualRecord.Name != null) ? individualRecord.Name.GivenName : String.Empty,
                                            LastName = (individualRecord.Name != null) ? individualRecord.Name.LastName : String.Empty,
                                            Sex = (Sex) Enum.Parse(typeof(Sex), individualRecord.Sex.ToString()),
                                            TreeId = DEFAULT_TREE_ID
                                        };

                ProcessFacts(individual, individualRecord.Events);

                ProcessMultimedia(individual, individualRecord.Multimedia);

                ProcessNotes(individual, individualRecord.Notes);

                ProcessCitations(individual, individualRecord.SourceCitations);

                Individuals.Add(individual);
            }
        }

        private void ProcessMultimedia(Entity entity, List<GEDCOMMultimediaStructure> multimedia)
        {
            foreach (var multimediaStructure in multimedia)
            {
                var multimediaLink = new MultimediaLink()
                                            {
                                                File = multimediaStructure.FileReference,
                                                Format = multimediaStructure.Format,
                                                Title = multimediaStructure.Title
//                                                OwnerType = (entity is Individual)
//                                                    ? EntityType.Individual
//                                                    : (entity is Fact)
//                                                        ? EntityType.Fact
//                                                        : (entity is Family)
//                                                            ? EntityType.Family
//                                                            : EntityType.Citation
                                            };


                entity.Multimedia.Add(multimediaLink);
            }
        }

        private void ProcessNote(Entity entity, string noteText)
        {
            if (!String.IsNullOrEmpty(noteText))
            {
                var newNote = new Note
                {
                    Text = noteText,
//                    OwnerId = entity.Id
                };
//                if (entity is Individual)
//                {
//                    newNote.OwnerType = EntityType.Individual;
//                }
//                else if (entity is Family)
//                {
//                    newNote.OwnerType = EntityType.Family;
//                }
//                else if (entity is Fact)
//                {
//                    newNote.OwnerType = EntityType.Fact;
//                }
//                else if (entity is Source)
//                {
//                    newNote.OwnerType = EntityType.Source;
//                }
//                else if (entity is Repository)
//                {
//                    newNote.OwnerType = EntityType.Repository;
//                }
                entity.Notes.Add(newNote);
            }
        }

        private void ProcessNotes(Entity entity, List<GEDCOMNoteStructure> notes)
        {
            foreach (var noteStructure in notes)
            {
                if (String.IsNullOrEmpty(noteStructure.XRefId))
                {
                    ProcessNote(entity, noteStructure.Text);
                }
                else
                {
                    if (_gedComDocument.NoteRecords[noteStructure.XRefId] is GEDCOMNoteRecord noteRecord && !String.IsNullOrEmpty(noteRecord.Data))
                    {

                        ProcessNote(entity, noteRecord.Data);
                    }
                }
            }
        }

        private  void LoadRepositories()
        {
            foreach (var gedcomRecord in _gedComDocument.RepositoryRecords)
            {
                var repositoryRecord = (GEDCOMRepositoryRecord)gedcomRecord;
                var repository = new Repository
                                        {
                                            // ReSharper disable once PossibleInvalidOperationException
                                            Id = repositoryRecord.GetId().Value,
                                            Address = repositoryRecord.Address != null ? repositoryRecord.Address.Address : "",
                                            Name = repositoryRecord.Name,
                                            TreeId = DEFAULT_TREE_ID
                                        };

                ProcessNotes(repository, repositoryRecord.Notes);

                Repositories.Add(repository);
            }
        }

        private  void LoadSources()
        {
            foreach (var gedcomRecord in _gedComDocument.SourceRecords)
            {
                var sourceRecord = (GEDCOMSourceRecord)gedcomRecord;
                var source = new Source
                                    {
                                        // ReSharper disable once PossibleInvalidOperationException
                                        Id = sourceRecord.GetId().Value,
                                        Author = sourceRecord.Author,
                                        Title = sourceRecord.Title,
                                        Publisher = sourceRecord.PublisherInfo,
                                        TreeId = DEFAULT_TREE_ID
                                    };
                if (sourceRecord.SourceRepository != null)
                {
                    source.RepositoryId = sourceRecord.SourceRepository.XRefId;
                }

                ProcessNotes(source, sourceRecord.Notes);

                Sources.Add(source);
            }
        }

        private  void LoadTree()
        {
            //Load tree meta data from Header Record
            
        }

        private static void RemoveIndividualFromFamilyRecord(Individual child, GEDCOMRecord familyRecord, GEDCOMTag tag)
        {
            var childRecord = (from GEDCOMRecord record in familyRecord.ChildRecords.GetLinesByTag(tag)
                               where record.XRefId == GEDCOMUtil.CreateId("I", child.Id)
                               select record).SingleOrDefault();

            if (childRecord != null)
            {
                familyRecord.ChildRecords.Remove(childRecord);
            }
        }

        private void UpdateFamilyDetails(Individual individual)
        {
            var familyRecord = _gedComDocument.SelectChildsFamilyRecord(GEDCOMUtil.CreateId("I", individual.Id));

            if (familyRecord != null)
            {
                if (individual.FatherId != GEDCOMUtil.GetId(familyRecord.Husband).ToString() || individual.MotherId != GEDCOMUtil.GetId(familyRecord.Wife).ToString())
                {
                    //remove child from current family
                    RemoveIndividualFromFamilyRecord(individual, familyRecord, GEDCOMTag.CHIL);

                    familyRecord = GetFamilyRecord(individual);

                    if (familyRecord != null)
                    {
                        //Add Individual as Child
                        familyRecord.AddChild(GEDCOMUtil.CreateId("I", individual.Id));
                    }
                    else
                    {
                        //new Family
                        CreateNewFamily(individual);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(individual.FatherId) || !string.IsNullOrEmpty(individual.MotherId))
                {
                    familyRecord = GetFamilyRecord(individual);

                    if (familyRecord != null)
                    {
                        //Add Individual as Child
                        familyRecord.AddChild(GEDCOMUtil.CreateId("I", individual.Id));
                    }
                    else
                    {
                        //new Family
                        CreateNewFamily(individual);
                    }
                }
            }
        }

        public void AddFamily(Family family)
        {
            Requires.NotNull("family", family);

            family.Id = _gedComDocument.Records.GetNextId(GEDCOMTag.FAM);

            var record = new GEDCOMFamilyRecord(family.Id.ToString());

            //Add HUSB
            if (!string.IsNullOrEmpty(family.HusbandId))
            {
                record.AddHusband(GEDCOMUtil.CreateId("I", family.HusbandId));
            }

            //Add WIFE
            if (!string.IsNullOrEmpty(family.WifeId))
            {
                record.AddWife(GEDCOMUtil.CreateId("I", family.WifeId));
            }

            foreach (Individual child in family.Children)
            {
                //Add CHIL
                record.AddChild(GEDCOMUtil.CreateId("I", child.Id));
            }

            _gedComDocument.AddRecord(record);
        }

        public void AddIndividual(Individual individual)
        {
            Requires.NotNull("individual", individual);

            //Add to internal List
            Individuals.Add(individual);

            //Add underlying GEDCOM record
            individual.Id = _gedComDocument.Records.GetNextId(GEDCOMTag.INDI);

            var record = new GEDCOMIndividualRecord(individual.Id.ToString());
            var name = new GEDCOMNameStructure(String.Format("{0} /{1}/", individual.FirstName, individual.LastName), record.Level + 1);

            record.Name = name;
            record.Sex = (Sex) Enum.Parse(typeof(Sex), individual.Sex.ToString());
            _gedComDocument.AddRecord(record);

            //Update Family Info
            UpdateFamilyDetails(individual);
        }

        public void AddRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public void AddSource(Source source)
        {
            throw new NotImplementedException();
        }

        public void AddTree(Tree tree)
        {
            throw new NotImplementedException();
        }

        public void DeleteFamily(Family family)
        {
            Requires.NotNull("family", family);

            GEDCOMFamilyRecord record = _gedComDocument.SelectFamilyRecord(GEDCOMUtil.CreateId("F", family.Id));

            if (record == null)
            {
                //record not in repository
                throw new ArgumentOutOfRangeException();
            }

            _gedComDocument.RemoveRecord(record);
        }

        public void DeleteIndividual(Individual individual)
        {
            Requires.NotNull("individual", individual);

            string individualId = GEDCOMUtil.CreateId("I", individual.Id);

            //Remove from internal List
            Individuals.Remove(individual);

            GEDCOMIndividualRecord record = _gedComDocument.SelectIndividualRecord(individualId);

            if (record == null)
            {
                //record not in repository
                throw new ArgumentOutOfRangeException();
            }

            _gedComDocument.RemoveRecord(record);

            //see if individual is a child in a family
            var familyRecord = _gedComDocument.SelectChildsFamilyRecord(individualId);
            if (familyRecord != null)
            {
                //remove child from family
                RemoveIndividualFromFamilyRecord(individual, familyRecord, GEDCOMTag.CHIL);
            }

            if (individual.Sex == Sex.Male)
            {
                //see if individual is a husband in a family
                foreach (GEDCOMFamilyRecord fRecord in _gedComDocument.SelectHusbandsFamilyRecords(individualId))
                {
                    //remove husband from family
                    RemoveIndividualFromFamilyRecord(individual, fRecord, GEDCOMTag.HUSB);
                }
            }
            else
            {
                //see if individual is a wife in a family
                foreach (GEDCOMFamilyRecord fRecord in _gedComDocument.SelectWifesFamilyRecords(individualId))
                {
                    //remove wife from family
                    RemoveIndividualFromFamilyRecord(individual, fRecord, GEDCOMTag.WIFE);
                }
            }
        }

        public void DeleteRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public void DeleteSource(Source source)
        {
            throw new NotImplementedException();
        }

        public void DeleteTree(Tree tree)
        {
            throw new NotImplementedException();
        }

        public void UpdateFamily(Family family)
        {
            Requires.NotNull("family", family);

            GEDCOMFamilyRecord record = _gedComDocument.SelectFamilyRecord(GEDCOMUtil.CreateId("F", family.Id));
            if (record == null)
            {
                //record not in repository
                throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateIndividual(Individual individual)
        {
            Requires.NotNull("individual", individual);

            GEDCOMIndividualRecord record = _gedComDocument.SelectIndividualRecord(GEDCOMUtil.CreateId("I", individual.Id));
            if (record == null)
            {
                //record not in repository
                throw new ArgumentOutOfRangeException();
            }

            record.Name = new GEDCOMNameStructure(String.Format("{0} /{1}/", individual.FirstName, individual.LastName), record.Level + 1);
            record.Sex = (Sex) Enum.Parse(typeof(Sex), individual.Sex.ToString());

            //Update Family Info
            UpdateFamilyDetails(individual);
        }

        public void UpdateRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public void UpdateSource(Source source)
        {
            throw new NotImplementedException();
        }

        public void UpdateTree(Tree tree)
        {
            throw new NotImplementedException();
        }
    }
}
