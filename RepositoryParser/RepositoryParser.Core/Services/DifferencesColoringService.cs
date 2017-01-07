﻿using System;
using System.Collections.Generic;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Configuration;

namespace RepositoryParser.Core.Services
{
    public class DifferencesColoringService : IDifferencesColoringService
    {
        public string ColorlessTextA { get; private set; }
        public string ColorlessTextB { get; private set; }
        public List<ChangesColorModel> TextAList { get; set; }
        public List<ChangesColorModel> TextBList { get; set; }

        public DifferencesColoringService(string texta, string textb)
        {
            this.ColorlessTextA = texta;
            this.ColorlessTextB = textb;
            TextAList = new List<ChangesColorModel>();
            TextBList = new List<ChangesColorModel>();
        }

        private ChangesColorModel LineColoring(string line1_output, string line2, bool isParent=false)
        {
            if (line1_output.StartsWith("+++") || line1_output.StartsWith("---") || line1_output.StartsWith("diff"))
                return new ChangesColorModel(line1_output, ChangeType.Unmodified);
            else if (line1_output.StartsWith("+") )
                return new ChangesColorModel(line1_output, ChangeType.Added);
            else if(line1_output.StartsWith("-"))
                return new ChangesColorModel(line1_output, ChangeType.Deleted);
      
            return new ChangesColorModel(line1_output, ChangeType.Unmodified);
            /* if ((!String.IsNullOrWhiteSpace(line1_output)) && (String.IsNullOrWhiteSpace(line2)))
             {
                 if(isParent)
                     return new ChangesColorModel(line1_output,ChangesColorModel.ChangeType.Deleted);
                 else
                     return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Added);
             }
             else if ((String.IsNullOrWhiteSpace(line1_output)) && (!String.IsNullOrWhiteSpace(line2)))
             {
                 if (isParent)
                     return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Added);
                 else
                     return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Deleted);
             }
             else if(line1_output==line2)
                 return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Unchanged);
             else if (
                 !(String.IsNullOrWhiteSpace(line1_output) && String.IsNullOrWhiteSpace(line2) &&
                   line1_output==line2))
                 return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Modified);
             else
                 return new ChangesColorModel(line1_output, ChangesColorModel.ChangeType.Unchanged);*/

        }

        private List<string> SplitChanges(string text)
        {
            List<string> textList = new List<string>();
            string[] array = text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            /*string[] array = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);*/
            foreach (string element in array)
            {
                textList.Add(element);
            }
            return textList;
        }


        public void FillColorDifferences()
        {
            if (!(String.IsNullOrEmpty(ColorlessTextA) && String.IsNullOrEmpty(ColorlessTextB)))
            {
                int iterations = 0;
                List<string> splitedText1 = SplitChanges(ColorlessTextA);
                List<string> splitedText2 = SplitChanges(ColorlessTextB);
                iterations = splitedText1.Count >= splitedText2.Count ? splitedText1.Count : splitedText2.Count;
                int text1count=splitedText1.Count;
                int text2count = splitedText2.Count;

                if (iterations == splitedText1.Count)
                {
                    for (int i = 0; i < (iterations - text2count); i++)
                        splitedText2.Add("");                  
                }
                else
                {
                    for (int i = 0; i < (iterations - text1count); i++)
                        splitedText1.Add("");                   
                }
                if (splitedText1.Count == splitedText2.Count)
                {
                    for (int i = 0; i < splitedText1.Count; i++)
                    {
                        TextAList.Add(LineColoring(splitedText1[i], splitedText2[i]));
                        TextBList.Add(LineColoring(splitedText2[i], splitedText1[i],true));
                    }
                }
            }
        }
    }
}
