﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RepositoryParser.Helpers;
using RepositoryParser.ViewModel.MonthActivityViewModels;
using RepositoryParser.ViewModel.UserActivityViewModels;

namespace RepositoryParser.View.UsersActivityViews
{
    /// <summary>
    /// Interaction logic for UserActivityFilesAnalyseView.xaml
    /// </summary>
    public partial class UserActivityFilesAnalyseView : UserControl
    {
        public UserActivityFilesAnalyseView()
        {
            InitializeComponent();
            ChartingHelper.Instance.DrawChart<UsersActivityFilesAnalyseViewModel>(this, this.ChartViewInstance);
        }
    }
}
