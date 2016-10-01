using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using De.TorstenMandelkow.MetroChart;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.ViewModel
{
    public abstract class ChartViewModelBase : RepositoryAnalyserViewModelBase
    {
        public List<ExtendedChartSeries> ExtendedChartSeries { get; protected set; }
        public ObservableCollection<ChartData> DataCollection { get; protected set; }
        public ObservableCollection<ChartLegendItemViewModel> ChartLegendItems { get; protected set; }
        public ChartBase ChartInstance { get; set; }

        public virtual void FillChartData()
        {
            ExtendedChartSeries = new List<ExtendedChartSeries>();
            DataCollection = new ObservableCollection<ChartData>();
            if (ChartLegendItems != null && ChartLegendItems.Any())
                ChartLegendItems.Clear();
            this.RaisePropertyChanged("DataCollection");
        }

        public void AddSeriesToChartInstance(string chartTitle, IEnumerable<ChartData> itemsSource)
        {
            ExtendedChartSeries.Add(new ExtendedChartSeries()
            {
                ChartTitle = chartTitle,
                ChartSeries = new ChartSeries()
                {
                    Caption = chartTitle,
                    DisplayMember = "ChartKey",
                    ValueMember = "ChartValue",
                    ItemsSource = null,
                    ToolTip = chartTitle
                },
                ItemsSource = itemsSource
            });
        }

        public void DrawChart()
        {
            ChartInstance.Series.Clear();
            ChartInstance.UpdateLayout();
            ExtendedChartSeries.ForEach(c =>
            {
                ChartInstance.Series.Add(c.ChartSeries);
                c.ChartSeries.ItemsSource = c.ItemsSource;
            });
            this.ChartLegendItems = new ObservableCollection<ChartLegendItemViewModel>(ChartInstance.ChartLegendItems);
            this.RaisePropertyChanged("ChartLegendItems");
        }

        public void FillDataCollection()
        {
            if (this.ExtendedChartSeries != null && this.ExtendedChartSeries.Any() && this.DataCollection != null)
            {
                DataCollection.Clear();
                ExtendedChartSeries.ForEach(c =>
                {
                    if (c.ItemsSource == null || !c.ItemsSource.Any())
                        return;

                    foreach (var item in c.ItemsSource)
                    {
                        if(item.ChartValue != 0)
                            DataCollection.Add(item);
                    }
                });
                RaisePropertyChanged("DataCollection");
            }
        }

        public int CountOfSelectedRepositories
        {
            get
            {
                return FilteringHelper.Instance.SelectedRepositories.Count;
            }
        }

        public async void ExportFile(string name) 
        {
            SaveFileDialog dlg = new SaveFileDialog();
            string newName = string.Empty;
            if (name.EndsWith("ViewModel",StringComparison.OrdinalIgnoreCase))
            {
               newName = name.Remove(name.Length - 9, 9);
            }
            if (string.IsNullOrEmpty(newName))
                newName = name;

            dlg.FileName = $"{newName}Chart_{DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss")}";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                DataToCsv.SaveChartReportToCsv(DataCollection,dlg.FileName);
                await DialogHelper.Instance.ShowDialog(new CustomDialogEntryData()
                {
                    MetroWindow = StaticServiceProvider.MetroWindowInstance,
                    DialogTitle = this.GetLocalizedString("Information"),
                    DialogMessage = this.GetLocalizedString("ExportMessage"),
                    OkButtonMessage = "Ok",
                    InformationType = InformationType.Information
                });
            }
        }

        private RelayCommand<object> _exportFileCommand;

        public RelayCommand<object> ExportFileCommand
        {
            get
            {
                return _exportFileCommand ?? (_exportFileCommand = new RelayCommand<object>((param) =>
                {
                    if (param == null)
                    {
                        this.ExportFile(this.GetType().Name);
                        return;
                    }
                    this.ExportFile(param.GetType().Name);
                }));
            }
        }
    }
}
