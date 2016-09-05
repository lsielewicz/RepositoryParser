using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Testing;
using NHibernate;
using NUnit.Framework;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.DataBaseTests.Configuration;

namespace RepositoryParser.DataBaseTests.Tests
{
    [TestFixture]
    public class MappingTest
    {
        private ISession _session;

        [SetUp]
        public void _Setup()
        {
            string dbLocalization = LocalizationConstants.TestDbLocalization;
            string dbDirectory = LocalizationConstants.TestDbDirectory;

            DbService.Instance.ChangeDataBaseLocation(dbLocalization, dbDirectory);

            _session = DbService.Instance.SessionFactory.OpenSession();
        }

        [Test]
        public void RepositoryEntityPropertiesMappingTest()
        {
            new PersistenceSpecification<Repository>(_session, new CustomEqualityComparer())
                .CheckProperty(p => p.Name, "SampleName")
                .CheckProperty(p => p.Type, "SampleType")
                .CheckProperty(p => p.Url, "Sample Url")
                .VerifyTheMappings();
        }

        [Test]
        public void BranchEnityPropertiesMappingTest()
        {
            new PersistenceSpecification<Branch>(_session)
                .CheckProperty(p => p.Name, "SampleName")
                .VerifyTheMappings();
        }

        [Test]
        public void CommitEnityPropertiesMappingTest()
        {
            new PersistenceSpecification<Commit>(_session)
                .CheckProperty(p => p.Author, "SampleAuthor")
                .CheckProperty(p => p.Date, DateTime.Now.Date)
                .CheckProperty(p => p.Email, "sampleemail@o2.pl")
                .CheckProperty(p => p.Message, "SampleMessage")
                .CheckProperty(p => p.Revision, Guid.NewGuid().ToString())
                .VerifyTheMappings();
        }

        [Test]
        public void ChangesEnityPropertiesMappingTest()
        {
            new PersistenceSpecification<Changes>(_session)
                .CheckProperty(p => p.ChangeContent, new string('S', 100000))
                .CheckProperty(p => p.Path, Environment.CurrentDirectory)
                .CheckProperty(p => p.Type, "Modified")
                .VerifyTheMappings();
        }
    }
}
