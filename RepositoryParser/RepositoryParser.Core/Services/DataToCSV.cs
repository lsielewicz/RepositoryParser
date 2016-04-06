﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Services
{
    public class DataToCsv
    {
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
        public static void CreateCSVFromGitCommitsList(List<CommitTable> data, string csvPath)
        {
            String csv = String.Join(
            Environment.NewLine,
            data.Select(d => d.ID + ";" + d.Message + ";" + d.Author + ";" + d.Date + ";" + d.Email)
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
    }
}
