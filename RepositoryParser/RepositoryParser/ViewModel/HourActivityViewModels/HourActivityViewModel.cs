using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using De.TorstenMandelkow.MetroChart;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NHibernate.Criterion;
using NHibernate.Util;
using RepositoryParser.Core.Messages;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.DataBaseManagementCore.Services;
using RepositoryParser.Helpers;


namespace RepositoryParser.ViewModel.HourActivityViewModels
{
    public class HourActivityViewModel : ChartViewModelBase
    {
        #region Variables
        private ObservableCollection<KeyValuePair<string, int>> _keyCollection;
        private ObservableCollection<ChartSeries> _chartSeriesCollection;
        private RelayCommand _exportFileCommand;
        #endregion

        public HourActivityViewModel()
        {
            KeyCollection = new ObservableCollection<KeyValuePair<string, int>>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        #region Getters/Setters
        public ObservableCollection<KeyValuePair<string, int>> KeyCollection
        {
            get { return _keyCollection; }
            set
            {
                if (_keyCollection != value)
                {
                    _keyCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Methods

        public override void FillChartData()
        {
            base.FillChartData();

            //todo loop around multiple repositories
            var itemSource = new List<ChartData>();
            for (int i = 0; i <= 23; i++)
            {
                using (var session = DbService.Instance.SessionFactory.OpenSession())
                {
                    var query = FilteringHelper.Instance.GenerateQuery(session);
                    var commitsCount =
                        query.Where(c => c.Date.Hour == i).Select(Projections.RowCount()).FutureValue<int>().Value;

                   itemSource.Add(new ChartData()
                   {
                       RepositoryValue = FilteringHelper.Instance.SelectedRepository,
                       ChartKey = TimeSpan.FromHours(i).ToString("hh':'mm"),
                       ChartValue = commitsCount
                   });
                }
            }

            this.AddSeriesToChartInstance(FilteringHelper.Instance.SelectedRepository,itemSource);
            this.DrawChart();
            this.FillDataCollection();
        }

        #endregion

                #region Buttons getters
            public
            RelayCommand ExportFileCommand
        {
            get { return _exportFileCommand ?? (_exportFileCommand = new RelayCommand(ExportFile)); }
        }
        #endregion

        #region Buttons actions
        public void ExportFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "MonthActivityChartView";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            // Show save file dialog box
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                Dictionary<string, int> tempDictionary = KeyCollection.ToDictionary(a => a.Key, a => a.Value);
                DataToCsv.CreateCSVFromDictionary(tempDictionary, filename);
                MessageBox.Show(ResourceManager.GetString("ExportMessage"), ResourceManager.GetString("ExportTitle"));
            }
        }
        #endregion
    }
}
