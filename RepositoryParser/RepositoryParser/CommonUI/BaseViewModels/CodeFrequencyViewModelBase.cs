using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RepositoryParser.CommonUI.CodeFrequency;
using RepositoryParser.Controls.MahAppsDialogOverloadings;
using RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog;
using RepositoryParser.Core.Enum;
using RepositoryParser.Core.Models;
using RepositoryParser.Core.Services;
using RepositoryParser.Helpers;

namespace RepositoryParser.CommonUI.BaseViewModels
{
    public abstract class CodeFrequencyViewModelBase : RepositoryAnalyserViewModelBase
    {
        public List<ExtendedChartSeries> AddedLinesChartList;
        public List<ExtendedChartSeries> DeletedLinesChartList;

        public CodeFrequencySubChartViewModel AddedChartViewModel { get; set; }
        public CodeFrequencySubChartViewModel DeletedChartViewModel { get; set; }

        public string SummaryString { get; set; }
        public ObservableCollection<CodeFrequencyDataRow> CodeFrequencyDataRows { get; private set; }

        public CodeFrequencyViewModelBase()
        {
            this.AddedLinesChartList = new List<ExtendedChartSeries>();
            this.DeletedLinesChartList = new List<ExtendedChartSeries>();
            this.AddedChartViewModel = new CodeFrequencySubChartViewModel();
            this.DeletedChartViewModel = new CodeFrequencySubChartViewModel();
            this.CodeFrequencyDataRows = new ObservableCollection<CodeFrequencyDataRow>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            this.FillData();
        }

        protected void ClearCollections()
        {
            if (this.AddedLinesChartList != null && this.AddedLinesChartList.Any())
                this.AddedLinesChartList.Clear();
            if (this.DeletedLinesChartList != null && this.DeletedLinesChartList.Any())
                this.DeletedLinesChartList.Clear();
            if (this.CodeFrequencyDataRows != null && this.CodeFrequencyDataRows.Any())
                this.CodeFrequencyDataRows.Clear();

        }

        private async void ExportFile(string name)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            string newName = string.Empty;
            if (name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
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
                DataToCsv.SaveCodeFrequecyToCsv(this.CodeFrequencyDataRows, dlg.FileName);
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

        public abstract void FillData();

        private RelayCommand _switchChartTypeCommand;
        public RelayCommand SwitchChartTypeCommand
        {
            get
            {
                return _switchChartTypeCommand ?? (_switchChartTypeCommand = new RelayCommand(() =>
                {
                    switch (this.AddedChartViewModel.CurrentChartType)
                    {
                        case ChartType.Primary:
                            this.AddedChartViewModel.CurrentChartType = ChartType.Secondary;
                            this.DeletedChartViewModel.CurrentChartType = ChartType.Secondary;
                            break;
                        case ChartType.Secondary:
                            this.AddedChartViewModel.CurrentChartType = ChartType.Primary;
                            this.DeletedChartViewModel.CurrentChartType = ChartType.Primary;
                            break;
                    }
                }));
            }
        }



    }
}
