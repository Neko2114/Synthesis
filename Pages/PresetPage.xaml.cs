using Penumbra_.Pages;
using Synthesis.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    public partial class PresetPage : Page
    {
        private static MainWindow window;
        public bool isMultiSelect;
        private string[][] presetData;
        public PresetPage(string title, bool multiSelect, int buttonStartIndex = 0)
        {
            InitializeComponent();

            window = (MainWindow)Application.Current.MainWindow;

            isMultiSelect = multiSelect;
            Title = title;
            PageHeader.Content = Title.ToUpper();

            presetData = DataManager.GetPresetData(title);

            next.Visibility = Visibility.Visible;
            finish.Visibility = Visibility.Hidden;

            ButtonGrid.SetupPresets(buttonGrid, this, presetData, buttonStartIndex);
            

            foreach (UIElement el in buttonGrid.Children)
            {
                Button btn = (Button)el;

                if (multiSelect)
                {
                    if (btn.Tag == null) btn.Tag = false;
                    btn.PreviewMouseLeftButtonUp += multiSelect_Click;
                }
                else
                {
                    btn.PreviewMouseLeftButtonUp += singleSelect_Click;
                }
            }
        }
        private void singleSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }
        private void multiSelect_Click(object sender, RoutedEventArgs e)
        {
            window.SetupSelection(sender as Button);

        }
        private void restartClick(object sender, System.Windows.RoutedEventArgs e)
        {
            window.SetupTagger();
        }
        private void nextBtn(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                if (isMultiSelect)
                {
                    var valueAdded = false;
                    foreach (UIElement el in buttonGrid.Children)
                    {
                        Button btn = (Button)el;
                        if ((bool)btn.Tag == true)
                        {
                            DataManager.GetTagList().Add(btn.Content.ToString().ToLower());
                            valueAdded = true;
                        }
                    }
                    if (valueAdded) DataManager.GetTagList().Add(Title);

                }
                window.NextPage();
            }
        }

        private void finishBtn(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                foreach (UIElement el in buttonGrid.Children)
                {
                    Button btn = (Button)el;
                    if ((bool)btn.Tag == true)
                    {
                        DataManager.GetTagList().Add(btn.Content.ToString().ToLower());
                    }
                }
                window.ViewTagResults();
            }
        }
    }
}