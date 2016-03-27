using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            int addedCounter, deletedCounter, unchangedCounter, modifiedCounter;
            addedCounter = deletedCounter = unchangedCounter = modifiedCounter = 0;

            childList.ForEach(x =>
            {
                if (x.Color == ChangesColorModel.ChangeType.Added)
                    addedCounter++;
                else if (x.Color == ChangesColorModel.ChangeType.Deleted)
                    deletedCounter++;
                else if (x.Color == ChangesColorModel.ChangeType.Modified && !String.IsNullOrWhiteSpace(x.Line))
                    modifiedCounter++;
                else if (x.Color == ChangesColorModel.ChangeType.Unchanged && !String.IsNullOrWhiteSpace(x.Line))
                    unchangedCounter++;
            });
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Added"),addedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Deleted"), deletedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Modified"), modifiedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Unchanged"), unchangedCounter));
        }
        #endregion
    }
}
