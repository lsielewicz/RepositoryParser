using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Configuration;


namespace RepositoryParser.ViewModel
{
    public class ChartOfChangesViewModel : RepositoryAnalyserViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _childCollection;
        #endregion

        public ChartOfChangesViewModel()
        { 
            Messenger.Default.Register<DataMessageToChartOfChanges>(this,x=>HandleDataMessage(x.ChildChangesList));
        }

        #region Getters/Setters
        public ObservableCollection<KeyValuePair<string, int>> ChildCollection
        {
            get { return _childCollection; }
            set
            {
                if (_childCollection != value)
                {
                    _childCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Messages
        private void HandleDataMessage(List<ChangesColorModel> childList)
        {
            ChildCollection=new ObservableCollection<KeyValuePair<string, int>>();
            int deletedCounter;
            var addedCounter = deletedCounter = 0;

            childList.ForEach(x =>
            {
                if (x.Color == ChangeType.Added)
                    addedCounter++;
                else if (x.Color == ChangeType.Deleted)
                    deletedCounter++;
            });
            ChildCollection.Add(new KeyValuePair<string, int>(ResourceManager.GetString("Added"),addedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(ResourceManager.GetString("Deleted"), deletedCounter));
        }
        #endregion
    }
}
