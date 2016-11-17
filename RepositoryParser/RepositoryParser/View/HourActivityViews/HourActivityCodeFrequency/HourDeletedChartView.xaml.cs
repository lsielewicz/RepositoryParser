﻿using System.Windows.Controls;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Helpers;

namespace RepositoryParser.View.HourActivityViews.HourActivityCodeFrequency
{
    /// <summary>
    /// Interaction logic for HourDeletedChartView.xaml
    /// </summary>
    public partial class HourDeletedChartView : UserControl
    {
        public HourDeletedChartView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                ChartingHelper.Instance.DrawCharts<CodeFrequencySubChartViewModel>(this,this.ChartViewInstance,this.ChartViewInstance2);
            };
        }
    }
}
