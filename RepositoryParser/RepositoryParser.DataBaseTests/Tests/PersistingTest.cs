using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Util;
using NUnit.Framework;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using LocalizationConstants = RepositoryParser.DataBaseTests.Configuration.LocalizationConstants;

namespace RepositoryParser.DataBaseTests.Tests
{
    [TestFixture]
    public class PersistingTest
    {

        [SetUp]
        public void _Setup()
        {
            string dbLocalization = LocalizationConstants.TestDbLocalization;
            string dbDirectory = LocalizationConstants.TestDbDirectory;

            DbService.Instance.ChangeDataBaseLocation(dbLocalization, dbDirectory);
        }

        [Test]
        public void PersistSmallPortionOfData()
        {
            Repository repository = new Repository()
            {
                Name = "SampleRepository",
                Type = RepositoryType.Git,
                Url = "http://github.com/lsielewicz/SampleRepository"
            };

            repository.AddBranch(new Branch(){Name = "First Branch"});
            repository.AddBranch(new Branch() {Name = "SecondBranch"});

            repository.Branches.ForEach(branch =>
            {
                for (int i = 0; i < 10; i++)
                {
                    branch.AddCommit(new Commit()
                    {
                        Author = "lsielewicz" + i,
                        Date = DateTime.Now.Date,
                        Email = "lsielewicz@o2.pl" +i,
                        Message = "This is sample commit"+i,
                        Revision = Guid.NewGuid().ToString()
                    });
                }
                branch.Commits.ForEach(commit =>
                {
                    for (int i = 0; i < 30; i++)
                    {
                        commit.AddChanges(new Changes()
                        {
                            ChangeContent = new string('T', 10000),
                            Path = "C:\\Windows\\"+i,
                            Type = "Modified " +i
                        });
                    }
                });
            });

            DbService.Instance.CreateDataBase();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(repository);
                    transaction.Commit();
                }
            }

            Repository persistedRepository;
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {

                     persistedRepository =
                        session.QueryOver<Repository>()
                            .Where(r => r.Name == "SampleRepository")
                            .SingleOrDefault<Repository>();


            }

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                Assert.AreEqual(persistedRepository.Name, repository.Name);
                Assert.AreEqual(persistedRepository.Type, repository.Type);
                Assert.AreEqual(persistedRepository.Url, repository.Url);
                Assert.AreEqual(persistedRepository.Branches.Count, repository.Branches.Count);
                for (int i = 0; i < repository.Branches.Count; i++)
                {
                    Assert.AreEqual(persistedRepository.Branches[i].Name, repository.Branches[i].Name);
                    Assert.AreEqual(persistedRepository.Branches[i].Commits.Count, repository.Branches[i].Commits.Count);
                    for (int j = 0; j < repository.Branches[i].Commits.Count; j++)
                    {
                        Assert.AreEqual(repository.Branches[i].Commits[j].Author, persistedRepository.Branches[i].Commits[j].Author);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Date, persistedRepository.Branches[i].Commits[j].Date);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Email, persistedRepository.Branches[i].Commits[j].Email);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Message, persistedRepository.Branches[i].Commits[j].Message);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Revision, persistedRepository.Branches[i].Commits[j].Revision);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Changes.Count, persistedRepository.Branches[i].Commits[j].Changes.Count);
                        for (int k = 0; k < repository.Branches[i].Commits[j].Changes.Count; k++)
                        {
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].ChangeContent, persistedRepository.Branches[i].Commits[j].Changes[k].ChangeContent);
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].Path, persistedRepository.Branches[i].Commits[j].Changes[k].Path);
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].Type, persistedRepository.Branches[i].Commits[j].Changes[k].Type);
                        }
                    }
                }
            }
    
        }

        [Test]
        public void PersistBigPortionOfData()
        {
            Repository repository = new Repository()
            {
                Name = "SampleRepository",
                Type = RepositoryType.Git,
                Url = "http://github.com/lsielewicz/SampleRepository"
            };

            repository.AddBranch(new Branch() { Name = "First Branch" });
            repository.AddBranch(new Branch() { Name = "SecondBranch" });

            repository.Branches.ForEach(branch =>
            {
                for (int i = 0; i < 100; i++)
                {
                    branch.AddCommit(new Commit()
                    {
                        Author = "lsielewicz" + i,
                        Date = DateTime.Now.Date,
                        Email = "lsielewicz@o2.pl" + i,
                        Message = "This is sample commit" + i,
                        Revision = Guid.NewGuid().ToString()
                    });
                }
                branch.Commits.ForEach(commit =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        commit.AddChanges(new Changes()
                        {
                            ChangeContent = new string('T', 50000),
                            Path = "C:\\Windows\\" + i,
                            Type = "Modified " + i
                        });
                    }
                });
            });

            DbService.Instance.CreateDataBase();
            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(repository);
                    transaction.Commit();
                }
            }


            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {

                var persistedRepository =
                    session.QueryOver<Repository>()
                        .Where(r => r.Name == "SampleRepository")
                        .SingleOrDefault<Repository>();

                Assert.AreEqual(persistedRepository.Name, repository.Name);
                Assert.AreEqual(persistedRepository.Type, repository.Type);
                Assert.AreEqual(persistedRepository.Url, repository.Url);
                Assert.AreEqual(persistedRepository.Branches.Count, repository.Branches.Count);
                for (int i = 0; i < repository.Branches.Count; i++)
                {
                    Assert.AreEqual(persistedRepository.Branches[i].Name, repository.Branches[i].Name);
                    Assert.AreEqual(persistedRepository.Branches[i].Commits.Count, repository.Branches[i].Commits.Count);
                    for (int j = 0; j < repository.Branches[i].Commits.Count; j++)
                    {
                        Assert.AreEqual(repository.Branches[i].Commits[j].Author, persistedRepository.Branches[i].Commits[j].Author);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Date, persistedRepository.Branches[i].Commits[j].Date);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Email, persistedRepository.Branches[i].Commits[j].Email);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Message, persistedRepository.Branches[i].Commits[j].Message);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Revision, persistedRepository.Branches[i].Commits[j].Revision);
                        Assert.AreEqual(repository.Branches[i].Commits[j].Changes.Count, persistedRepository.Branches[i].Commits[j].Changes.Count);
                        for (int k = 0; k < repository.Branches[i].Commits[j].Changes.Count; k++)
                        {
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].ChangeContent, persistedRepository.Branches[i].Commits[j].Changes[k].ChangeContent);
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].Path, persistedRepository.Branches[i].Commits[j].Changes[k].Path);
                            Assert.AreEqual(repository.Branches[i].Commits[j].Changes[k].Type, persistedRepository.Branches[i].Commits[j].Changes[k].Type);
                        }
                    }
                }

            }
        }
    }
}
