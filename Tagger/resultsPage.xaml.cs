using Synthesis.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    public partial class resultsPage : Page
    {
        MainWindow window;
        public resultsPage(bool isRepeat = false)
        {
            InitializeComponent();
            window = (MainWindow)Application.Current.MainWindow;
            var pathHash = new HashSet<string>();

            List<string> tags = null;
            if (isRepeat)
            {
                tags = DataManager.GetRecentTags();
            }
            else
            {
                tags = DataManager.GetTagList();
            }

            foreach (var tag in tags)
            {
                listResults.Items.Add(tag);
                var pathList = DataManager.GetMatchingPaths(tag);

                foreach (var path in pathList)
                {
                    pathHash.Add(path);
                }
            }

            pathHash = pathHash.OrderBy(x => x).Cast<string>().ToHashSet();

            foreach (var path in pathHash)
            {
                var listBtn = new Button() { Content = path };
                listBtn.PreviewMouseLeftButtonUp += pathListBtn_Click;
                pathList.Items.Add(listBtn);
            }

            if (pathHash.Count <= 0)
            {
                var listBtn = new Button() { IsEnabled = false, Content = "None found" };
                pathList.Items.Add(listBtn);
            }

        }
        private void pathListBtn_Click(object sender, RoutedEventArgs e)
        {
            window.repeatPreviousTags.IsEnabled = false;
            window.refreshTags.IsEnabled = true;
            window.IsSortMode = false;
            IOManager.SaveSortPathLocally((sender as Button).Content.ToString(), DataManager.GetSelectedItem().objectName);
        }
        private void restartClick(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            window.SetupTagger();
        }
    }
}
