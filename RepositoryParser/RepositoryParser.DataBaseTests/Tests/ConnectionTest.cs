using System.IO;
using NUnit.Framework;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.DataBaseTests.Configuration;

namespace RepositoryParser.DataBaseTests.Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        [Test]
        public void TestConnection()
        {
            string dbLocalization = LocalizationConstants.TestDbLocalization;
            string dbDirectory = LocalizationConstants.TestDbDirectory;

            DbService.Instance.ChangeDataBaseLocation(dbLocalization,dbDirectory);

            Assert.IsTrue(Directory.Exists(dbDirectory));
            Assert.IsTrue(File.Exists(dbLocalization));
            Assert.That(DbService.Instance, Is.Not.Null);
            Assert.That(DbService.Instance.SessionFactory, Is.Not.Null);
        }

    }
}
