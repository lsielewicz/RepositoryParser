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
        private ObservableCollection<KeyValuePair<string, int>> childCollection;
        private ResourceManager _resourceManager;
        public ChartOfChangesViewModel()
        {
        _resourceManager = new ResourceManager("RepositoryParser.Properties.Resources", Assembly.GetExecutingAssembly());
        Messenger.Default.Register<DataMessageToChartOfChanges>(this,x=>HandleDataMessage(x.ChildChangesList));
        }

        public ObservableCollection<KeyValuePair<string, int>> ChildCollection
        {
            get { return childCollection; }
            set
            {
                if (childCollection != value)
                {
                    childCollection = value;
                    RaisePropertyChanged("ChildCollection");
                }
            }
        }



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
                else if (x.Color == ChangesColorModel.ChangeType.Modified)
                    modifiedCounter++;
                else
                    unchangedCounter++;
            });
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Added"),addedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Deleted"), deletedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Modified"), modifiedCounter));
            ChildCollection.Add(new KeyValuePair<string, int>(_resourceManager.GetString("Unchanged"), unchangedCounter));

            
        }
    }
}
