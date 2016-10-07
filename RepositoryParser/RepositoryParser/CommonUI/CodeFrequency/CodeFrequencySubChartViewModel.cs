using System.Collections.Generic;
using System.Linq;
using RepositoryParser.CommonUI.BaseViewModels;
using RepositoryParser.Core.Models;

namespace RepositoryParser.CommonUI.CodeFrequency
{
    public class CodeFrequencySubChartViewModel : ChartViewModelBase
    {
        public override void FillChartData()
        {
            if (this.ExtendedChartSeries == null)
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
    }
}
