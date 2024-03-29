﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeHeader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        string GetExeDirPath()
        {
            string myExePath = System.Reflection.Assembly.GetEntryAssembly().Location; // 切換本exe的目錄
            string myExeDir = System.IO.Path.GetDirectoryName(myExePath);
            return myExeDir;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.ShowNewFolderButton = false;
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.SelectedPath))
            {
                TextBox_FolderPath.Text = dialog.SelectedPath;
            }
        }

        bool CheckCommentExist(ref string allTxt, bool removeThem = false)
        {
            bool isExist = false;
            Regex reg = new Regex(string.Format(@"{0}---[\s\S]*{0}---.*[\r\n]*", this.TextBox_CommentSymbol.Text));

            var match = reg.Match(allTxt);

            if (match != null && match.Success && match.Groups.Count == 1)
            {
                Group g = match.Groups[0];
                if (g.Captures[0].Index < 10)
                {
                    isExist = true;
                    if (removeThem)
                    {
                        string capStr = g.Captures[0].ToString();
                        allTxt = allTxt.Replace(capStr, "");
                    }
                }
            }

            return isExist;
        }
        private void BtnTryAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (string filePath in Directory.GetFiles(this.TextBox_FolderPath.Text, this.TextBox_SearchPattern.Text, SearchOption.AllDirectories))
            {
                string fileTxt = File.ReadAllText(filePath);

                bool isExist = CheckCommentExist(ref fileTxt, false);

                if (!isExist)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("{0}------------------------------------------------------------------------------", this.TextBox_CommentSymbol.Text));
                    string[] groupContent = this.TextBox_CommentContent.Text.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
                    foreach (string gc in groupContent)
                    {
                        sb.AppendLine(this.TextBox_CommentSymbol.Text + gc);
                    }
                    sb.AppendLine(string.Format("{0}------------------------------------------------------------------------------", this.TextBox_CommentSymbol.Text));

                    // Add
                    string comment = sb.ToString();

                    fileTxt = comment + fileTxt;

                    File.WriteAllText(filePath, fileTxt, Encoding.UTF8);

                    Console.WriteLine("Add Comment TO File: " + filePath);
                }
            }
        }

        private void BtnTryRemove_Click(object sender, RoutedEventArgs e)
        {
            foreach (string filePath in Directory.GetFiles(this.TextBox_FolderPath.Text, this.TextBox_SearchPattern.Text, SearchOption.AllDirectories))
            {
                string fileTxt = File.ReadAllText(filePath);
                bool isExist = CheckCommentExist(ref fileTxt, true);

                if (isExist)
                {
                    File.WriteAllText(filePath, fileTxt, Encoding.UTF8);
                    Console.WriteLine("Remove Comment TO File: " + filePath);
                }

            }
        }

    }
}
