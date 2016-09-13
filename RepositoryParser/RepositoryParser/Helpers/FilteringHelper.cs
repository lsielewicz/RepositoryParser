using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Helpers
{
    public class FilteringHelper
    {
        #region singleton
        private static FilteringHelper _instance;
        public static FilteringHelper Instance
        {
            get
            {
                return _instance ?? (_instance = new FilteringHelper()); 
            }
        }
        private FilteringHelper()
        {
            
        }
        #endregion

        public string SelectedRepository { get; set; }
        public string SelectedBranch { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string MessageCriteria { get; set; }
        public string SelectedAuthor { get; set; }

        public void Initialize()
        {
            if(_instance == null)
                _instance = new FilteringHelper();
        }

        public IQueryOver<Commit, Commit> GenerateQuery(ISession session)
        {
            bool isCountiuned = false;

            Repository repository = null;
            Branch branch = null;

            var query = session.QueryOver<Commit>();
            if (!string.IsNullOrEmpty(SelectedRepository) && !string.IsNullOrEmpty(SelectedBranch))
            {
                query =
                    query.JoinAlias(commit => commit.Branches, () => branch, JoinType.LeftOuterJoin)
                        .JoinAlias(() => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                        .Where(() => branch.Name == SelectedBranch && repository.Name == SelectedRepository)
                        .TransformUsing(Transformers.DistinctRootEntity);

                isCountiuned = true;
            }
            else if (!string.IsNullOrEmpty(SelectedRepository) &&
                        string.IsNullOrEmpty(SelectedBranch))
            {
                query =
                    query.JoinAlias(commit => commit.Branches, () => branch, JoinType.LeftOuterJoin)
                        .JoinAlias(() => branch.Repository, () => repository, JoinType.LeftOuterJoin)
                        .Where(() => repository.Name == SelectedRepository)
                        .TransformUsing(Transformers.DistinctRootEntity);
                isCountiuned = true;
            }

            bool isAuthor = !string.IsNullOrEmpty(SelectedAuthor);
            bool isFromDate = !string.IsNullOrEmpty(DateFrom);
            bool isToDate = !string.IsNullOrEmpty(DateTo);
            bool isMessage = !string.IsNullOrEmpty(MessageCriteria);

            if (!isAuthor && !isFromDate && !isToDate && !isMessage&&
                !isCountiuned)
                return session.QueryOver<Commit>();
            if (!isAuthor && !isFromDate && !isToDate && !isMessage)
            {
                query =
                    query.Where(
                            () =>
                                branch.Name == SelectedBranch ||
                                branch.Name == "trunk" && repository.Name == SelectedRepository)
                        .TransformUsing(Transformers.DistinctRootEntity);
                return query;
            }
            if (!isAuthor && !isFromDate && !isToDate)
            {
                query = query.Where(Restrictions.On<Commit>(c => c.Message).IsLike(MessageCriteria, MatchMode.Anywhere));
                return query;
            }

            if (isAuthor && !isFromDate && !isToDate)
            {
                query = query.Where(commit => commit.Author == SelectedAuthor);
            }
            else if (isAuthor && isFromDate && isToDate)
            {
                query =
                    query.Where(
                        commit =>
                            commit.Author == SelectedAuthor && commit.Date >= DateTime.Parse(DateFrom) &&
                            commit.Date <= DateTime.Parse(DateTo));
            }
            else if (isAuthor && isFromDate)
            {
                query =
                    query.Where(
                        commit => commit.Author == SelectedAuthor && commit.Date >= DateTime.Parse(DateFrom)); //todo check
            }
            else if (isAuthor)
            {
                query =
                    query.Where(
                        commit => commit.Author == SelectedAuthor && commit.Date <= DateTime.Parse(DateTo));
                ;
            }
            else if (isFromDate && isToDate)
            {
                query =
                    query.Where(
                        commit => commit.Date >= DateTime.Parse(DateFrom) && commit.Date <= DateTime.Parse(DateTo));
            }
            else if (isFromDate)
            {
                query = query.Where(commit => commit.Date >= DateTime.Parse(DateFrom));
            }
            else
            {
                query = query.Where(commit => commit.Date <= DateTime.Parse(DateTo));
            }

            if (isMessage)
            {
                query = query.Where(Restrictions.On<Commit>(c => c.Message).IsLike(MessageCriteria, MatchMode.Anywhere));
            }

            return query;
        }

    }
}
