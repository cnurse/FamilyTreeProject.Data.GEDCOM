using System;
using System.Linq;
using FamilyTreeProject.Data.GEDCOM.Tests.Common;
using NUnit.Framework;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable UseStringInterpolation

namespace FamilyTreeProject.Data.GEDCOM.Tests
{
    [TestFixture]
    public class GEDCOMStoreCommonTests : GEDCOMTestBase
    {
        [Test]
        public void GEDCOMStore_Constructor_Throws_On_Empty_Path()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => { new GEDCOMFileStore(""); });
        }

        [Test]
        [TestCase("NoRecords", 0)]
        [TestCase("OneIndividual", 1)]
        [TestCase("TwoIndividuals", 2)]
        public void GEDCOMStore_Constructor_Loads_Individuals_Property(string fileName, int recordCount)
        {
            //Arrange
            const string testFile = "Constructor.ged";
            var db = CreateStore(String.Format("{0}.ged", fileName), testFile);

            var inds = db.Individuals;
            Assert.AreEqual(recordCount, inds.Count);
        }

        [Test]
        [TestCase("NoRecords", 0)]
        [TestCase("OneFamily", 1)]
        [TestCase("TwoFamilies", 2)]
        public void GEDCOMStore_Constructor_Loads_Families_Property(string fileName, int recordCount)
        {
            //Arrange
            const string testFile = "Constructor.ged";
            var db = CreateStore(string.Format("{0}.ged", fileName), testFile);

            var families = db.Families;
            Assert.AreEqual(recordCount, families.Count);
        }

        [Test]
        public void GEDCOMStore_Constructor_Creates_Family_Links()
        {
            //Arrange
            const string testFile = "Constructor.ged";
            const string fileName = "BindingTest";
            var db = CreateStore(String.Format("{0}.ged", fileName), testFile);

            //Act
            var testIndividual = db.Individuals.SingleOrDefault(ind => ind.Id == 1);

            //Assert
            if (testIndividual != null)
            {
                Assert.AreEqual("John", testIndividual.FirstName);
                Assert.AreEqual("Smith", testIndividual.LastName);
                Assert.AreEqual("@I2@", testIndividual.FatherId);
                Assert.AreEqual("@I3@", testIndividual.MotherId);
            }
        }


    }
}