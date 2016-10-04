using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Core.Models;

namespace RepositoryParser.ViewModel.UserActivityViewModels.UsersActivityCodeFrequency
{
    public class UsersAddedChartViewModel : ChartViewModelBase
    {
        public override void FillChartData()
        {
            if(this.ExtendedChartSeries ==null)
                base.FillChartData();        
        }

        public void RedrawChart(List<ExtendedChartSeries> chartSeries)
        {
            this.ExtendedChartSeries = chartSeries;
            if (this.ExtendedChartSeries != null && this.ExtendedChartSeries.Any())
            {
                this.DrawChart();
                this.FillDataCollection();
            }
        }

        public int CountOfAuthors
        {
            get
            {
                return ViewModelLocator.Instance.Filtering.SelectedAuthors.Count != 0
                    ? ViewModelLocator.Instance.Filtering.SelectedAuthors.Count
                    : ViewModelLocator.Instance.Filtering.AuthorsCollection.Count;
            }
        }
    }
}
