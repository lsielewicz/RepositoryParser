using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;

namespace RepositoryParser.ViewModel
{
    public class DifferenceWindowViewModel : ViewModelBase
    {
        #region Variables
        private GitRepository gitRepository;
        private string filteringQuery;
        private ObservableCollection<KeyValuePair<int, string>> commitsCollection;
        private KeyValuePair<int, string> selectedItem;
        private KeyValuePair<string, string> changeSelectedItem;
        private ObservableCollection<KeyValuePair<string, string>> changesCollection;
        private string textA;
        private string textB;
        private string changeQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        #endregion
        #region Constructors

        public DifferenceWindowViewModel()
        {
            Messenger.Default.Register<DataMessageToCharts>(this, x => HandleChartMessage(x.RepoInstance, x.FilteringQuery));
            CommitsCollection = new ObservableCollection<KeyValuePair<int, string>>();
            ChangesCollection = new ObservableCollection<KeyValuePair<string, string>>();
        }
        #endregion
        #region Methods

        private void HandleChartMessage(GitRepository repo, string query)
        {
            this.gitRepository = repo;
            this.filteringQuery = query;
            FillContent();
        }

        private void FillContent()
        {
            if (CommitsCollection != null && CommitsCollection.Count > 0)
                CommitsCollection.Clear();

            string query;
            if (string.IsNullOrEmpty(filteringQuery))
            {
                query = "SELECT * From GitCommits";
            }
            else
            {
                query = filteringQuery;
                SQLiteCommand command = new SQLiteCommand(query, gitRepository.SqLiteInstance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string message = Convert.ToString(reader["Message"]);
                    KeyValuePair<int, string> dictionary = new KeyValuePair<int, string>(id, message);
                    commitsCollection.Add(dictionary);
                }
            }
        }
        #endregion
        #region Getters/Setters

        public string TextA
        {
            get
            {
                return textA;

            }
            set
            {
                if (textA != value)
                {
                    textA = value;
                    RaisePropertyChanged("TextA");
                }
            }
        }

        public string TextB
        {
            get
            {
                return textB;

            }
            set
            {
                if (textB != value)
                {
                    textB = value;
                    RaisePropertyChanged("TextB");
                }
            }
        }

        public ObservableCollection<KeyValuePair<int, string>> CommitsCollection
        {
            get
            {
                return commitsCollection;

            }
            set
            {
                if (commitsCollection != value)
                {
                    commitsCollection = value;
                    RaisePropertyChanged("CommitsCollection");
                }
            }
        }
        public ObservableCollection<KeyValuePair<string, string>> ChangesCollection
        {
            get
            {
                return changesCollection;

            }
            set
            {
                if (changesCollection != value)
                {
                    changesCollection = value;
                    RaisePropertyChanged("ChangesCollection");
                }
            }
        }

        public KeyValuePair<int, string> SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                selection(selectedItem);
                RaisePropertyChanged("SelectedItem");
            }
        }

        public KeyValuePair<string, string> ChangeSelectedItem
        {
            get
            {
                return changeSelectedItem;

            }
            set
            {
                changeSelectedItem = value;
                ChangeSelection(changeSelectedItem);
                RaisePropertyChanged("ChangeSelectedItem");
            }
        }

        private void ChangeSelection(KeyValuePair<string, string> dic)
        {
            if (!string.IsNullOrEmpty(changeQuery))
            {
                TextA = "";
                TextB = "";
                string query = changeQuery + " and Changes.Type ='" + dic.Key + "' and Changes.Path='" + dic.Value + "'";
                SQLiteCommand command = new SQLiteCommand(query, gitRepository.SqLiteInstance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                string texta = "";
                string textb = "";
                while (reader.Read())
                {
                    texta = Convert.ToString(reader["TextA"]);
                    textb = Convert.ToString(reader["TextB"]);
                }
                TextA = texta;
                TextB = textb;
            }
        }
        private void selection(KeyValuePair<int, string> dictionary)
        {
            if (ChangesCollection != null && ChangesCollection.Count > 0)
                ChangesCollection.Clear();
            string query =
                "Select * from Changes inner join ChangesForCommit on Changes.ID=ChangesForCommit.NR_Change where " +
                "ChangesForCommit.NR_Commit=" + dictionary.Key;
            changeQuery = query;
            SQLiteCommand command = new SQLiteCommand(query, gitRepository.SqLiteInstance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string type = Convert.ToString(reader["Type"]);
                string path = Convert.ToString(reader["Path"]);
                KeyValuePair<string, string> values = new KeyValuePair<string, string>(type, path);
                ChangesCollection.Add(values);
            }

        }
        #endregion
    }
}
