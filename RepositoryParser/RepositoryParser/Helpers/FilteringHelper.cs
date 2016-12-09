using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using RepositoryParser.Core.Models;
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
            SelectedRepositories = new List<string>();
            SelectedAuthors = new List<string>();
        }
        #endregion

        public List<string> SelectedAuthors { get; set; }
        public List<string> SelectedRepositories { get; set; }
        public string SelectedBranch { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string MessageCriteria { get; set; }

        public static int CountOfRepositories
        {
            get
            {
                return Instance.SelectedRepositories?.Count ?? 0;
            }
        }

        public void Initialize()
        {
            if(_instance == null)
                _instance = new FilteringHelper();
        }

        public IQueryOver<Commit, Commit> GenerateQuery(ISession session, string selectedRepository = "")
        {
            if(!string.IsNullOrEmpty(selectedRepository))
                return GenerateQueryForSingleRepository(session,selectedRepository);
            return GenerateQueryForMultipleRepositories(session);   
        }

        private IQueryOver<Commit, Commit> GenerateQueryForMultipleRepositories(ISession session)
        {
            Repository repository = null;
            Branch branch = null;
            var query = session.QueryOver<Commit>();

            if (this.SelectedRepositories != null && this.SelectedRepositories.Any())
            {
                query =
                    query.JoinAlias(c => c.Branches, () => branch, JoinType.InnerJoin)
                        .JoinAlias(() => branch.Repository, () => repository, JoinType.InnerJoin)
                        .WhereRestrictionOn(() => repository.Name)
                        .IsIn(this.SelectedRepositories).TransformUsing(Transformers.DistinctRootEntity);
            }
            if (this.SelectedRepositories != null && this.SelectedRepositories.Count == 1)
            {
                query = query.Where(() => branch.Name == SelectedBranch).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (this.SelectedAuthors != null && this.SelectedAuthors.Any())
            {
                query = query.WhereRestrictionOn(c => c.Author).IsIn(this.SelectedAuthors).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.DateFrom))
            {
                query = query.Where(commit => commit.Date >= DateTime.Parse(DateFrom)).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.DateTo))
            {
                query = query.Where(commit => commit.Date <= DateTime.Parse(DateTo)).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.MessageCriteria))
            {
                query = query.Where(Restrictions.On<Commit>(c => c.Message).IsLike(MessageCriteria, MatchMode.Anywhere)).TransformUsing(Transformers.DistinctRootEntity);
            }

            return query;
        }

        private IQueryOver<Commit, Commit> GenerateQueryForSingleRepository(ISession session,string selectedRepository)
        {
            Repository repository = null;
            Branch branch = null;
            var query = session.QueryOver<Commit>();

            if (this.SelectedRepositories != null && this.SelectedRepositories.Any())
            {
                query =
                    query.JoinAlias(c => c.Branches, () => branch, JoinType.InnerJoin)
                        .JoinAlias(() => branch.Repository, () => repository, JoinType.InnerJoin)
                        .Where(() => repository.Name == selectedRepository).TransformUsing(Transformers.DistinctRootEntity);
            }
            if (this.SelectedRepositories != null && this.SelectedRepositories.Count == 1)
            {
                query = query.Where(() => branch.Name == SelectedBranch).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (this.SelectedAuthors != null && this.SelectedAuthors.Any())
            {
                query = query.WhereRestrictionOn(c => c.Author).IsIn(this.SelectedAuthors).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.DateFrom))
            {
                query = query.Where(commit => commit.Date >= DateTime.Parse(DateFrom)).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.DateTo))
            {
                query = query.Where(commit => commit.Date <= DateTime.Parse(DateTo)).TransformUsing(Transformers.DistinctRootEntity);
            }

            if (!string.IsNullOrEmpty(this.MessageCriteria))
            {
                query = query.Where(Restrictions.On<Commit>(c => c.Message).IsLike(MessageCriteria, MatchMode.Anywhere)).TransformUsing(Transformers.DistinctRootEntity);
            }

            return query;
        }


        public void SaveChartReportToCsv(ObservableCollection<ChartData> data, string pathToSave, string title = "")
        {

            string csv = $"{title}{Environment.NewLine}Date,{DateTime.Now.Date}{Environment.NewLine}{Environment.NewLine}Key,Value,Repository{Environment.NewLine}";
            csv += string.Join(Environment.NewLine,
                data.Select(d => d.ChartKey + "," + d.ChartValue + "," + d.RepositoryValue));

            csv += $"{Environment.NewLine}{Environment.NewLine}Filtering" +
                   $"{Environment.NewLine}Repositories," +
                   string.Join(",", this.SelectedRepositories) +
                   $"{Environment.NewLine}Date:,{this.DateFrom ?? "Undefined"},-,{this.DateTo ?? "Undefined"}" +
                   $"{Environment.NewLine}Message criteria:,{this.MessageCriteria ?? "None"}";

            if (this.SelectedAuthors.Any())
            {
                csv += $"{Environment.NewLine}Authors:," + string.Join(",", this.SelectedAuthors);
            }
            else
            {
                csv += $"{Environment.NewLine}Authors:,All";
            }
            

            File.WriteAllText(pathToSave, csv, Encoding.UTF8);
        }


    }
}
