﻿using System.Windows.Controls;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.WeekdayActivityViewModels;

namespace RepositoryParser.View.WeekdayActivityViews
{
    /// <summary>
    /// Interaction logic for WeekdayActivityContiniousAnalyseView.xaml
    /// </summary>
    public partial class WeekdayActivityContiniousAnalyseView : UserControl
    {
        public WeekdayActivityContiniousAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawCharts<WeekdayActivityContiniousAnalyseViewModel>(this,this.ChartViewInstance, this.ChartViewInstance2);
        }
    }
}
