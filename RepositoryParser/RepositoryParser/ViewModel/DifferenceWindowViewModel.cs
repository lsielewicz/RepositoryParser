using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NHibernate.Util;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Entities;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;
using RepositoryParser.View;

namespace RepositoryParser.ViewModel
{
    public class DifferenceWindowViewModel : RepositoryAnalyserViewModelBase
    {
        #region Variables
        private ObservableCollection<KeyValuePair<int, string>> _commitsCollection;
        private KeyValuePair<int, string> _commitSelectedItem;
        private Changes _changeSelectedItem;
        private ObservableCollection<Changes> _changesCollection;
        private string _changePatch;
        private DifferencesColoringService _colorService;
        private ObservableCollection<ChangesColorModel> _changePatchcollection;
        private RelayCommand _goToChartOfChangesCommand;

        private readonly BackgroundWorker _showDifferencesWorker;
        private readonly BackgroundWorker _onLoadWorker;
        #endregion

        #region Constructors

        public DifferenceWindowViewModel()
        {
            ChangesCollection = new ObservableCollection<Changes>();

            this._showDifferencesWorker = new BackgroundWorker();
            this._showDifferencesWorker.DoWork += this.ShowDifferencesWork;
            this._showDifferencesWorker.RunWorkerCompleted += this.ShowDifferencesCompleted;

            this._onLoadWorker = new BackgroundWorker();
            this._onLoadWorker.DoWork += this.OnLoadWork;
            this._onLoadWorker.RunWorkerCompleted += this.OnLoadWorkCompleted;

        }

        #endregion

        #region Methods
        private void ClearViewModel()
        {
            if (CommitsCollection != null && CommitsCollection.Any())
                CommitsCollection.Clear();
            if (ChangesCollection != null && ChangesCollection.Any())
                ChangesCollection.Clear();
            if (ChangePatchCollection != null && ChangePatchCollection.Any())
                ChangePatchCollection.Clear();

            CommitSelectedItem = new KeyValuePair<int, string>();
            ChangeSelectedItem = new Changes();

        }

        private void ChangeSelection(Changes changes)
        {
            if (changes == null)
                return;
            ChangePatch = string.Empty;

            _colorService = new DifferencesColoringService(changes.ChangeContent, string.Empty);
            _colorService.FillColorDifferences();

            ChangePatchCollection = new ObservableCollection<ChangesColorModel>();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                int startedIndex = 0;
                if (_colorService != null && _colorService.TextAList.Count > 3 && Regex.IsMatch(_colorService.TextAList.First().Line, "diff --git") && Regex.IsMatch(_colorService.TextAList[3].Line,"Binary files"))
                    startedIndex = 3;
                else if (_colorService != null && _colorService.TextAList.Count > 3 && Regex.IsMatch(_colorService.TextAList.First().Line, "diff --git") && !Regex.IsMatch(_colorService.TextAList[3].Line, "Binary files"))
                    startedIndex = 5;

                foreach (var item in _colorService.TextAList.Skip(startedIndex))
                    ChangePatchCollection.Add(item);
            }));


            Messenger.Default.Send<DataMessageToChartOfChanges>(new DataMessageToChartOfChanges(_colorService.TextAList));

        }
        private void CommitSelection(KeyValuePair<int, string> dictionary)
        {
            if (ChangesCollection != null && ChangesCollection.Count > 0)
                ChangesCollection.Clear();
            if (ChangePatchCollection != null && ChangePatchCollection.Any())
                ChangePatchCollection.Clear();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var changes = session.QueryOver<Changes>().Where(change => change.Commit.Id == dictionary.Key).List<Changes>();
                changes.ForEach(change =>
                {
                    ChangesCollection.Add(change);
                });
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

        public ObservableCollection<ChangesColorModel> ChangePatchCollection
        {
            get { return _changePatchcollection;}
            set
            {
                if (_changePatchcollection != value)
                {
                    _changePatchcollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ChangePatch
        {
            get
            {
                return _changePatch;

            }
            set
            {
                if (_changePatch != value)
                {
                    _changePatch = value;
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<Changes> ChangesCollection
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
                    RaisePropertyChanged();
                }
            }
        }

        public KeyValuePair<int, string> CommitSelectedItem
        {
            get
            {
                return _commitSelectedItem;
            }
            set
            {
                if (_commitSelectedItem.Key != value.Key && _commitSelectedItem.Value != value.Value)
                {
                    _commitSelectedItem = new KeyValuePair<int, string>(value.Key,value.Value);
                    CommitSelection(_commitSelectedItem);
                    RaisePropertyChanged();
                }
                
            }
        }

        public Changes ChangeSelectedItem
        {
            get
            {
                return _changeSelectedItem;
            }
            set
            {
                _changeSelectedItem = value;
                if(!_showDifferencesWorker.IsBusy)
                    _showDifferencesWorker.RunWorkerAsync(_changeSelectedItem);
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Buttons
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
            Application.Current.Dispatcher.InvokeAsync(new Action(() => { IsLoading = true; }), DispatcherPriority.Send);
            Changes arg = (Changes) e.Argument;
            ChangeSelection(arg);
        }

        private void ShowDifferencesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
        }


        private void OnLoadWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(new Action(() => { IsLoading = true; }),DispatcherPriority.Send);
            CommitsCollection=new ObservableCollection<KeyValuePair<int, string>>();

            using (var session = DbService.Instance.SessionFactory.OpenSession())
            {
                var commits = FilteringHelper.Instance.GenerateQuery(session).List<Commit>();
                commits.ForEach(
                    commit =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CommitsCollection.Add(new KeyValuePair<int, string>(commit.Id, commit.Message));
                        });
                    });
            }
        }

        private void OnLoadWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
        }

        public override void OnLoad()
        {
            ClearViewModel();
            if (!_onLoadWorker.IsBusy)
                _onLoadWorker.RunWorkerAsync();
        }
        #endregion

       
    }
}
