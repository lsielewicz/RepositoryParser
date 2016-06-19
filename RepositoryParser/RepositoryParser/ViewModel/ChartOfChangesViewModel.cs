using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Enums;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;


namespace RepositoryParser.ViewModel
{
    public class ChartOfChangesViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<KeyValuePair<string, int>> _childCollection;
        private ResourceManager _resourceManager;
        #endregion

        public ChartOfChangesViewModel()
        {
            _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
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
                    RaisePropertyChanged("ChildCollection");
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
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Added"),addedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Deleted"), deletedCounter));
        }
        #endregion
    }
}
