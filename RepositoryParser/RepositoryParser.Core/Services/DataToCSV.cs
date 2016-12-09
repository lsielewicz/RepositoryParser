﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Core.Services
{
    public class DataToCsv
    {
        //todo GENERIC !!
        public static void CreateCSVFromDictionary(Dictionary<string, int> data, string csvPath)
        {
            String csv = String.Join(
                Environment.NewLine,
                data.Select(d => d.Key + ";\t" + d.Value)
                );
            File.WriteAllText(csvPath, csv, Encoding.UTF8);
        }

        public static void CreateCSVFromDictionary(Dictionary<int, int> data, string csvPath)
        {
            String csv = String.Join(
                Environment.NewLine,
                data.Select(d => d.Key + ";\t" + d.Value)
                );
            File.WriteAllText(csvPath, csv, Encoding.UTF8);
        }

        public static void CreateCSVFromGitCommitsList(List<Commit> data, string csvPath)
        {
            String csv = String.Join(
                Environment.NewLine,
                data.Select(d => d.Id + ";" + d.Message + ";" + d.Author + ";" + d.Date + ";" + d.Email)
                );
            File.WriteAllText(csvPath, csv, Encoding.UTF8);
        }

        public static void CreateSummaryChartCSV(List<UserCodeFrequency> data, string data2, string csvPath)
        {
            String csv = String.Join(Environment.NewLine, data2);
            csv = String.Join(
                Environment.NewLine,
                data.Select(d => d.User + ";" + d.AddedLines + ";" + d.DeletedLines)
                );
            File.WriteAllText(csvPath, csv, Encoding.UTF8);
        }

        public static void SaveChartReportToCsv(ObservableCollection<ChartData> data, string pathToSave, string title="")
        {

            string csv = $"{title}{Environment.NewLine}Date,{DateTime.Now.Date}{Environment.NewLine}{Environment.NewLine}Key,Value,Repository{Environment.NewLine}";
            csv += string.Join(Environment.NewLine,
                data.Select(d => d.ChartKey + "," + d.ChartValue + "," + d.RepositoryValue));

            //csv += $"{Environment.NewLine}Filtering{Environment.NewLine}Repositories" + string.Join(",",)

            File.WriteAllText(pathToSave, csv, Encoding.UTF8);
        }

        public static void SaveCodeFrequecyToCsv(IList<CodeFrequencyDataRow> data, string pathToSave)
        {
            string csv = "ChartKey;Added lines;Deleted lines; Repository" + Environment.NewLine;
            csv += string.Join(Environment.NewLine,
                data.Select(d => d.ChartKey + ";" + d.AddedLines + ";" + d.DeletedLines + ";" + d.Repository));
            File.WriteAllText(pathToSave, csv, Encoding.UTF8);
        }
}
}
