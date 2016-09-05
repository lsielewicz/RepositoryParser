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
        public void RepositoryEntityMappingTest()
        {
            new PersistenceSpecification<Repository>(_session)
                .CheckProperty(p => p.Name, "SampleName")
                .CheckProperty(p => p.Type, "SampleType")
                .CheckProperty(p => p.Url, "Sample Url")
                .VerifyTheMappings();
        }
    }
}
