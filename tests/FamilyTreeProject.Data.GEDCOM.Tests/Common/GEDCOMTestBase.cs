using System.IO;
using System.Reflection;
using FamilyTreeProject.Common;
using FamilyTreeProject.Common.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace FamilyTreeProject.Data.GEDCOM.Tests.Common
{
    public abstract class GEDCOMTestBase
    {
        private IConfigurationRoot _configuration;
        
        private string TreeId => _configuration["treeId"];
        private string FirstName => _configuration["firstName"];
        private  Sex IndividualsSex => _configuration["sex"] == "Male" ? Sex.Male : Sex.Female;
        private string LastName => _configuration["lastName"];

        private string EmbeddedFilePath => _configuration["embeddedFilePath"];
        
        protected string FilePath { get; private set; }

        [OneTimeSetUp]
        public void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("testSettings.json");
            
            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory)));

           
            _configuration = builder.Build();
            
            FilePath = Path.Combine(solutionDir, _configuration["filePath"]);           
        }

         protected GEDCOMFileStore CreateStore(string file, string test)
        {
            string fileName = Path.Combine(FilePath, file);
            string testFile = Path.Combine(FilePath, test);
            File.Copy(fileName, testFile, true);

            return new GEDCOMFileStore(testFile);
        }
        
        protected Family CreateTestFamily(int id = -1)
        {
            // Create a test family
            var newFamily = new Family
            {
                Id = id,
                TreeId = TreeId
            };

            // Return the family
            return newFamily;
        }

        protected Individual CreateTestIndividual()
        {
            return CreateTestIndividual(-1);
        }

        protected Individual CreateTestIndividual(int id)
        {
            // Create a test individual
            var newIndividual = new Individual
            {
                Id = id,
                FirstName = FirstName,
                LastName = LastName,
                Sex = IndividualsSex,
                TreeId = TreeId
            };

            // Return the individual
            return newIndividual;
        }
        
        protected string GetEmbeddedFileName(string fileName)
        {
            string fullName = $"{EmbeddedFilePath}.{fileName}";
            if (!fullName.ToLower().EndsWith(".ged"))
            {
                fullName += ".ged";
            }

            return fullName;
        }

        protected Stream GetEmbeddedFileStream(string fileName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(GetEmbeddedFileName(fileName));
        }

        protected string GetEmbeddedFileString(string fileName)
        {
            string text = "";
            using (var reader = new StreamReader(GetEmbeddedFileStream(fileName)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    text += $"{line}\n";
                }
            }
            return text;
        }

        private string GetFileName(string fileName)
        {
            string fullName = Path.Combine(FilePath, fileName);
            if (!fullName.ToLower().EndsWith(".ged"))
            {
                fullName += ".ged";
            }

            return fullName;
        }

        protected string GetFileString(string fileName)
        {
            string text = "";
            using (StreamReader reader = new StreamReader(new FileStream(GetFileName(fileName), FileMode.Open, FileAccess.Read)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    text += $"{line}\n";
                }
            }
            return text;
        }
    }
}