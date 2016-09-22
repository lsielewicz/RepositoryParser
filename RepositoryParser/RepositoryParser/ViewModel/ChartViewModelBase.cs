using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using De.TorstenMandelkow.MetroChart;
using RepositoryParser.Core.Models;
using RepositoryParser.Helpers;
using EnumerableExtensions = NHibernate.Util.EnumerableExtensions;

namespace RepositoryParser.ViewModel
{
    public abstract class ChartViewModelBase : RepositoryAnalyserViewModelBase
    {
        public List<ExtendedChartSeries> ExtendedChartSeries { get; protected set; }
        public ObservableCollection<ChartData> DataCollection { get; protected set; }
        public ChartBase ChartInstance { get; set; }

        public virtual void FillChartData()
        {
            ExtendedChartSeries = new List<ExtendedChartSeries>();
            DataCollection = new ObservableCollection<ChartData>();
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
                    ItemsSource = null
                },
                ItemsSource = itemsSource
            });
        }

        public void DrawChart()
        {
            ExtendedChartSeries.ForEach(c =>
            {
                ChartInstance.Series.Add(c.ChartSeries);
                c.ChartSeries.ItemsSource = c.ItemsSource;
            });
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
                        DataCollection.Add(item);
                    }
                });
                RaisePropertyChanged("DataCollection");
            }
        
        }
    }
}
