using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.View;

namespace RepositoryParser.ViewModel
{
    public class DifferenceWindowViewModel : ViewModelBase
    {
        #region Variables
        private ObservableCollection<KeyValuePair<int, string>> _commitsCollection;
        private KeyValuePair<int, string> _selectedItem;
        private KeyValuePair<string, string> _changeSelectedItem;
        private ObservableCollection<KeyValuePair<string, string>> _changesCollection;
        private string _textA;
        private string _textB;
        private string _changeQuery;
        private string _filteringQuery;
        private ResourceManager _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        private DifferencesColoringService _colorService;
        private ObservableCollection<ChangesColorModel> _listTextA;
        private RelayCommand _goToChartOfChangesCommand;
        private RelayCommand _closedEventCommand;
        private bool _progressBarVisibility;


        private BackgroundWorker _showDifferencesWorker;
        private BackgroundWorker _onLoadWorker;
        #endregion

        #region Constructors

        public DifferenceWindowViewModel()
        {
            Messenger.Default.Register<ChartMessageLevel2>(this, x => HandleChartMessage(x.FilteringQuery));
            ChangesCollection = new ObservableCollection<KeyValuePair<string, string>>();

            this._showDifferencesWorker = new BackgroundWorker();
            this._showDifferencesWorker.DoWork += this.ShowDifferencesWork;
            this._showDifferencesWorker.RunWorkerCompleted += this.ShowDifferencesCompleted;

            this._onLoadWorker = new BackgroundWorker();
            this._onLoadWorker.DoWork += this.OnLoadWork;
            this._onLoadWorker.RunWorkerCompleted += this.OnLoadWorkCompleted;

        }
        #endregion

        #region Methods
        private void ClosedEvent()
        {
            if (CommitsCollection != null)
                CommitsCollection.Clear();
            if (ChangesCollection != null)
                ChangesCollection.Clear();
            if (ListTextA != null)
                ListTextA.Clear();

            SelectedItem = new KeyValuePair<int, string>();
            ChangeSelectedItem = new KeyValuePair<string, string>();

        }
        private void HandleChartMessage(string query)
        {
            ClosedEvent();

            this._filteringQuery = query;
            if(!_onLoadWorker.IsBusy)
                _onLoadWorker.RunWorkerAsync();
        }

        private void ChangeSelection(KeyValuePair<string, string> dic)
        {
            if (!string.IsNullOrEmpty(_changeQuery))
            {
                TextA = "";
                TextB = "";
                string query = _changeQuery + " and Changes.Type ='" + dic.Key + "' and Changes.Path='" + dic.Value + "'";
                SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
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

                _colorService = new DifferencesColoringService(TextA, TextB);
                _colorService.FillColorDifferences();

                ListTextA = new ObservableCollection<ChangesColorModel>();
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    int startedIndex = 0;
                    if (_colorService != null && _colorService.TextAList.Count > 3 && Regex.IsMatch(_colorService.TextAList.First().Line, "diff --git") && Regex.IsMatch(_colorService.TextAList[3].Line,"Binary files"))
                        startedIndex = 3;
                    else if (_colorService != null && _colorService.TextAList.Count > 3 && Regex.IsMatch(_colorService.TextAList.First().Line, "diff --git") && !Regex.IsMatch(_colorService.TextAList[3].Line, "Binary files"))
                        startedIndex = 5;

                    //  _colorService.TextAList.ForEach(x => ListTextA.Add(x));
                    foreach (var item in _colorService.TextAList.Skip(startedIndex))
                        ListTextA.Add(item);
                }));


                Messenger.Default.Send<DataMessageToChartOfChanges>(new DataMessageToChartOfChanges(_colorService.TextAList));
            }
        }
        private void selection(KeyValuePair<int, string> dictionary)
        {
            if (ChangesCollection != null && ChangesCollection.Count > 0)
                ChangesCollection.Clear();
            string query =
                "Select * from Changes inner join ChangesForCommit on Changes.ID=ChangesForCommit.NR_Change where " +
                "ChangesForCommit.NR_Commit=" + dictionary.Key;
            _changeQuery = query;
            SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string type = Convert.ToString(reader["Type"]);
                string path = Convert.ToString(reader["Path"]);
                KeyValuePair<string, string> values = new KeyValuePair<string, string>(type, path);
                ChangesCollection.Add(values);
            }

        }

        private void GoToChartOfChanges()
        {
            ChartOfChangesView _window = new ChartOfChangesView();
            if (_colorService != null)
                Messenger.Default.Send<DataMessageToChartOfChanges>(new DataMessageToChartOfChanges(_colorService.TextAList));
            _window.Show();
        }

        #endregion

        #region Getters/Setters
        public bool ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                if (_progressBarVisibility != value)
                    _progressBarVisibility = value;
                RaisePropertyChanged("ProgressBarVisibility");
            }
        }
        public ObservableCollection<ChangesColorModel> ListTextA
        {
            get { return _listTextA;}
            set
            {
                if (_listTextA != value)
                {
                    _listTextA = value;
                    RaisePropertyChanged("ListTextA");
                }
            }
        }

        public string TextA
        {
            get
            {
                return _textA;

            }
            set
            {
                if (_textA != value)
                {
                    _textA = value;
                    RaisePropertyChanged("TextA");
                }
            }
        }

        public string TextB
        {
            get
            {
                return _textB;

            }
            set
            {
                if (_textB != value)
                {
                    _textB = value;
                    RaisePropertyChanged("TextB");
                }
            }
        }

        public ObservableCollection<KeyValuePair<int, string>> CommitsCollection
        {
            get
            {
                return _commitsCollection;

            }
            set
            {
                if (_commitsCollection != value)
                {
                    _commitsCollection = value;
                    RaisePropertyChanged("CommitsCollection");
                }
            }
        }
        public ObservableCollection<KeyValuePair<string, string>> ChangesCollection
        {
            get
            {
                return _changesCollection;

            }
            set
            {
                if (_changesCollection != value)
                {
                    _changesCollection = value;
                    RaisePropertyChanged("ChangesCollection");
                }
            }
        }

        public KeyValuePair<int, string> SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem.Key != value.Key && _selectedItem.Value != value.Value)
                {
                    _selectedItem = new KeyValuePair<int, string>(value.Key,value.Value);
                    selection(_selectedItem);
                    RaisePropertyChanged("SelectedItem");
                }
                
            }
        }

        public KeyValuePair<string, string> ChangeSelectedItem
        {
            get
            {
                return _changeSelectedItem;
            }
            set
            {
                _changeSelectedItem = value;
                //ChangeSelection(changeSelectedItem);
                if(!_showDifferencesWorker.IsBusy)
                    _showDifferencesWorker.RunWorkerAsync(_changeSelectedItem);
                RaisePropertyChanged("ChangeSelectedItem");
            }
        }




        #endregion

        #region Buttons
        public RelayCommand ClosedEventCommand
        {
            get { return _closedEventCommand ?? (_closedEventCommand = new RelayCommand(ClosedEvent)); }
        }

        public RelayCommand GoToChartOfChangesCommand
        {
            get
            {
                return _goToChartOfChangesCommand ?? (_goToChartOfChangesCommand = new RelayCommand(GoToChartOfChanges));
            }
        }
        #endregion

        #region backgroundworker
        private void ShowDifferencesWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(new Action(() => { ProgressBarVisibility = true; }), DispatcherPriority.Send);
            KeyValuePair<string, string> arg = (KeyValuePair<string, string>) e.Argument;
            ChangeSelection(arg);
        }

        private void ShowDifferencesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarVisibility = false;
        }


        private void OnLoadWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(new Action(() => { ProgressBarVisibility = true; }),DispatcherPriority.Send);
            CommitsCollection=new ObservableCollection<KeyValuePair<int, string>>();
            
            string query;
            if (string.IsNullOrEmpty(_filteringQuery))
            {
                query = "SELECT * From Commits";
            }
            else
            {
                query = _filteringQuery;
            }
            SQLiteCommand command = new SQLiteCommand(query, SqLiteService.GetInstance().Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string message = Convert.ToString(reader["Message"]);
                    KeyValuePair<int, string> dictionary = new KeyValuePair<int, string>(id, message);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CommitsCollection.Add(dictionary);
                    });
                    
                 }
            ProgressBarVisibility = false;
        }

        private void OnLoadWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }



        #endregion

       
    }
}
