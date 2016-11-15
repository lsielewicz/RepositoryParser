using System.Windows.Controls;
using De.TorstenMandelkow.MetroChart;
using RepositoryParser.CommonUI.BaseViewModels;

namespace RepositoryParser.Helpers
{
    public class ChartingHelper
    {
        private static ChartingHelper _instance;

        public static ChartingHelper Instance
        {
            get { return _instance ?? (_instance = new ChartingHelper()); }
        }

        private ChartingHelper() { }

        public void DrawChart<T>(ContentControl view, ChartBase chartBase) where T : ChartViewModelBase
        {
            var viewModel = view.DataContext as T;
            if (viewModel != null && chartBase != null)
            {
                viewModel.PrimaryChartInstance = chartBase;
                viewModel.FillChartData();
            }
        }

        public void DrawCharts<T>(ContentControl view, ChartBase primaryChartbase, ChartBase secondaryChartBase) where T : ChartViewModelBase
        {
            var viewModel = view.DataContext as T;
            if (viewModel != null && primaryChartbase != null && secondaryChartBase != null)
            {
                viewModel.PrimaryChartInstance = primaryChartbase;
                viewModel.SecondaryChartInstance = secondaryChartBase;
                viewModel.FillChartData();
            }
        }
    }
}
