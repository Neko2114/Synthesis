using Synthesis.Managers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    public partial class modPage : Page
    {
        private static List<ContentPage> additionalGrids = new List<ContentPage>();
        private static List<Grid> contentList;
        MainWindow window;
        public modPage()
        {
            InitializeComponent();
            window = (MainWindow)Application.Current.MainWindow;
        }
        private void modSelect(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            contentList = new List<Grid>();
            var title = btn.Content.ToString().ToLower();
            var GRID_SIZE_MAX = 42;
            var indexStart = 0;
            var pageDataSize = 0;

            if (!title.Equals("mixed"))
            {
                if (title.Equals("presets"))
                {
                    pageDataSize = DataManager.GetPresetDataSize(title);
                    indexStart = 0;
                    while (pageDataSize >= indexStart)
                    {
                        contentList.Add((Grid)new PresetPage(title, false, indexStart).Content);
                        // Add 42 buttons per full grid.
                        indexStart += 42;
                    }
                }
                else
                {
                    pageDataSize = DataManager.GetPageDataSize(title);
                    indexStart = 0;
                    while (pageDataSize >= indexStart)
                    {
                        if (title.Equals("shoes")
                            || (title.Equals("options")))
                        {
                            contentList.Add((Grid)new ContentPage(title, true, indexStart).Content);
                        }
                        else
                        {
                            contentList.Add((Grid)new ContentPage(title, false, indexStart).Content);
                        }
                        // Add 42 buttons per full grid.
                        indexStart += 42;
                    }
                }
            }
            else
            {
                var mixed = new string[] { "head", "body", "hands", "legs", "shoes", "accessories" };
                foreach (var type in mixed)
                {
                    indexStart = 0;
                    pageDataSize = DataManager.GetPageDataSize(type);
                    while (pageDataSize >= indexStart)
                    {
                        if (type.Equals("shoes"))
                        {
                            contentList.Add((Grid)new ContentPage(type, true, indexStart).Content);
                        }
                        else
                        {
                            contentList.Add((Grid)new ContentPage(type, false, indexStart).Content);
                        }
                        // Add 42 buttons per full grid.
                        indexStart += 42;
                    }
                }
            }
            contentList.Add((Grid)new ContentPage("extras", true).Content);

            window.SetupPages(contentList);
            window.NextPage();
        }
        public static void addExtraPages(ContentPage page)
        {
            additionalGrids.Add(page);
        }
        public static int GetTotalExtraButtons()
        {
            return (additionalGrids.Count * 42) + 42;
        }
        private void extras(object sender, RoutedEventArgs e)
        {
            var contentList = new List<Grid>();
            contentList.Add((Grid)new ContentPage("extras", true).Content);

            window.SetupPages(contentList);
            window.NextPage();
        }
    }
}
