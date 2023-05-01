using Penumbra_.Pages;
using Synthesis.Managers;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    public partial class ContentPage : Page
    {
        private static MainWindow window;
        public bool isMultiSelect;
        private string[] data;
        public ContentPage(string title, bool multiSelect, int buttonStartIndex = 0)
        {
            InitializeComponent();

            window = (MainWindow)Application.Current.MainWindow;

            isMultiSelect = multiSelect;
            Title = title;
            PageHeader.Content = Title.ToUpper();

            data = DataManager.GetPageData(title);

            if (Title.Equals("extras"))
            {
                next.Visibility = Visibility.Hidden;
                finish.Visibility = Visibility.Visible;
            }
            else
            {
                next.Visibility = Visibility.Visible;
                finish.Visibility = Visibility.Hidden;
            }

            ButtonGrid.SetupModButtons(buttonGrid, this, data, buttonStartIndex);

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
            Button btn = sender as Button;
            DataManager.GetTagList().Add(Title);
            DataManager.GetTagList().Add(btn.Content.ToString().ToLower());
            window.NextPage();
        }
        public void multiSelect_Click(object sender, RoutedEventArgs e)
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