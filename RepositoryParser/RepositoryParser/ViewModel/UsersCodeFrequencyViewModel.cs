using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;

namespace RepositoryParser.ViewModel
{
    public class UsersCodeFrequencyViewModel : ViewModelBase
    {
        #region Fields
        private string _filteringQuery;
        private SqLiteService _sqLiteService;
        private DifferencesColoringService _colorService;
        private List<UserCodeFrequency> _userCodeFreqList;
        #endregion

        public UsersCodeFrequencyViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this,x=>HandleDataMessage(x.FilteringQuery));
            _sqLiteService=SqLiteService.GetInstance();
        }




        #region Messages
        private void HandleDataMessage(string filteringQuery)
        {
            this._filteringQuery = filteringQuery;
            FillCollections();
        }
        #endregion


        #region Methods
        private void FillCollections()
        {
            _userCodeFreqList=new List<UserCodeFrequency>();
            List<string> authorsList = GetAuthors(_filteringQuery);
            int added, deleted, modified;

            for (int i = 0; i < authorsList.Count; i++)
            {
                added = deleted = modified = 0;
                if (authorsList[i] == "")
                    continue;
                string query = "select * from Commits ";
                if (string.IsNullOrEmpty(MatchQuery(_filteringQuery)))
                {
                    query += "where Commits.Author='" + authorsList[i] + "' ";
                }
                else
                {
                    if (authorsList.Count == 1)
                        query += MatchQuery(_filteringQuery);
                    else
                        query += MatchQuery(_filteringQuery) + "and Commits.Author='" + authorsList[i] + "' ";
                }

                SQLiteCommand command = new SQLiteCommand(query, _sqLiteService.Connection);
                SQLiteDataReader reader = command.ExecuteReader();

                List<int> idCommitList = new List<int>();
                if (reader.Read())
                {
                    idCommitList.Add(Convert.ToInt32(reader["ID"]));      
                }

                foreach (int idCommit in idCommitList)
                {
                    string query2 =
                        String.Format(
                            "select * from Changes " +
                            "inner join ChangesForCommit on Changes.ID = ChangesForCommit.NR_Change " +
                            "inner join Commits on ChangesForCommit.NR_Commit " +
                            "where Commits.ID = {0}",
                            idCommit);
    

                    SQLiteCommand command2 = new SQLiteCommand(query2, _sqLiteService.Connection);
                    SQLiteDataReader reader2 = command2.ExecuteReader();

                    List<ChangesTable> changesList = new List<ChangesTable>();
                    if (reader2.Read())
                    {
                        int id = Convert.ToInt32(reader["ID"]);
                        string type = Convert.ToString(reader["Type"]);
                        string path = Convert.ToString(reader["Path"]);
                        string textA = Convert.ToString(reader["TextA"]);
                        string textB = Convert.ToString(reader["TextB"]);
                        
                        changesList.Add(new ChangesTable(id,type,path,textA,textB));
                    }
                    if (changesList.Count > 0)
                    {
                        foreach (var change in changesList)
                        {
                            _colorService = new DifferencesColoringService(change.TextA,change.TextB);
                            _colorService.FillColorDifferences();

                            _colorService.TextAList.ForEach(x =>
                            {
                                if (x.Color == ChangesColorModel.ChangeType.Added)
                                    added++;
                                else if (x.Color == ChangesColorModel.ChangeType.Deleted)
                                    deleted++;
                                else if (x.Color == ChangesColorModel.ChangeType.Modified && !String.IsNullOrWhiteSpace(x.Line))
                                    modified++;
                            });
                        }
                        
                    }
                }
                _userCodeFreqList.Add(new UserCodeFrequency(authorsList[i],added,deleted,modified));
            }
        }

        private string MatchQuery(string query)
        {
            Regex r = new Regex(@"(select \* from Commits)(.*)", RegexOptions.IgnoreCase);

            Match m = r.Match(query);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                {
                    query = m.Groups[2].Value;
                }
            }
            return query;
        }

        private List<string> GetAuthors(string query)
        {
            List<string> newAuthorsList = new List<string>();
            query = "SELECT Author FROM Commits " + MatchQuery(query) + "Group by Author";

            SQLiteCommand command = new SQLiteCommand(query, _sqLiteService.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                newAuthorsList.Add(Convert.ToString(reader["Author"]));
            }
            return newAuthorsList;
        }
        #endregion
    }
}
