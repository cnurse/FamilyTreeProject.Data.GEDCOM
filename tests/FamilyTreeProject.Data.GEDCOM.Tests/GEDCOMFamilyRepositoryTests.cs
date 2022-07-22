﻿using System;
using System.Collections.Generic;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using Moq;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement

namespace FamilyTreeProject.Data.GEDCOM.Tests
{
    [TestFixture]
    public class GEDCOMFamilyRepositoryTests
    {
        [Test]
        public void Constructor_Throws_On_Null_Database()
        {
            //Arrange
            IFileStore store = null;

            //Act

            //Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => new FamilyRepository(store));
        }

        [Test]
        public void Add_Throws_On_Null_Family()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Add(null));
        }

        [Test]
        public void Add_Calls_Store_AddFamily()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);
            var family = new Family();

            //Act
            rep.Add(family);

            //Assert
            mockStore.Verify(s => s.AddFamily(family));
        }

        [Test]
        public void Delete_Throws_On_Null_Family()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Delete(null));
        }

        [Test]
        public void Delete_Calls_Store_DeleteFamily()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);
            var family = new Family();

            //Act
            rep.Delete(family);

            //Assert
            mockStore.Verify(s => s.DeleteFamily(family));
        }

        [Test]
        public void GetAll_Calls_Store_Families()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            mockStore.Setup(s => s.Families).Returns(() => new List<Family>());
            var rep = new FamilyRepository(mockStore.Object);

            //Act
            rep.GetAll();

            //Assert
            mockStore.Verify(s => s.Families);
        }

        [Test]
        public void Update_Throws_On_Null_Family()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => rep.Update(null));
        }

        [Test]
        public void Update_Calls_Store_UpdateFamily()
        {
            //Arrange
            var mockStore = new Mock<IFileStore>();
            var rep = new FamilyRepository(mockStore.Object);
            var family = new Family();

            //Act
            rep.Update(family);

            //Assert
            mockStore.Verify(s => s.UpdateFamily(family));
        }
    }
}
