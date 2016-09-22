using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using De.TorstenMandelkow.MetroChart;

namespace RepositoryParser.Core.Models
{
    public class ExtendedChartSeries
    {
        public string ChartTitle { get; set; }
        public ChartSeries ChartSeries { get; set; }
        public IEnumerable<ChartData> ItemsSource { get; set; }
    }
}
