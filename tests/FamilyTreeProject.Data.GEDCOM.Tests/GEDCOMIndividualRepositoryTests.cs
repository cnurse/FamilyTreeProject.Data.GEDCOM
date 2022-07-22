using System;
using System.Collections.Generic;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using Moq;
using NUnit.Framework;

namespace FamilyTreeProject.Data.GEDCOM.Tests
{
    [TestFixture]
    public class GEDCOMIndividualRepositoryTests
    {
        [Test]
        public void Constructor_Throws_On_Null_Database()
        {
            //Arrange
            IFileStore store = null;

            //Act

            //Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new IndividualRepository(store));
        }

        [Test]
        public void Add_Throws_On_Null_Individual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Add(null));
        }

        [Test]
        public void Add_Calls_Store_AddIndividual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);
            var individual = new Individual();

            //Act
            rep.Add(individual);

            //Assert
            mockStore.Verify(s => s.AddIndividual(individual));
        }

        [Test]
        public void Delete_Throws_On_Null_Individual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Delete(null));
        }

        [Test]
        public void Delete_Calls_Store_DeleteIndividual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);
            var individual = new Individual();

            //Act
            rep.Delete(individual);

            //Assert
            mockStore.Verify(s => s.DeleteIndividual(individual));
        }

        [Test]
        public void GetAll_Calls_Store_Individuals()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            mockStore.Setup(s => s.Individuals).Returns(() => new List<Individual>());
            var rep = new IndividualRepository(mockStore.Object);

            //Act
            rep.GetAll();

            //Assert
            mockStore.Verify(s => s.Individuals);
        }

        [Test]
        public void Update_Throws_On_Null_Individual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Update(null));
        }

        [Test]
        public void Update_Calls_Store_UpdateIndividual()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new IndividualRepository(mockStore.Object);
            var individual = new Individual();

            //Act
            rep.Update(individual);

            //Assert
            mockStore.Verify(s => s.UpdateIndividual(individual));
        }
    }
}
