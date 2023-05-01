using MahApps.Metro.Controls;
using Microsoft.Win32;
using Synthesis.Managers;
using Synthesis.Penumbra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Synthesis.Managers.DataManager;
using Ookii.Dialogs.Wpf;

namespace Synthesis
{
    public partial class MainWindow : MetroWindow
    {
        private static string searchString = string.Empty;
        private bool IsMenuInteraction = false;
        public static ListBox ExplorerList;
        private static MainWindow mainWindow;
        public bool IsSortMode = false;

        public MainWindow()
        {
            mainWindow = this;
            InitializeComponent();
            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        /* ----------------------------- MainWindow Functions  -----------------------------
        */
        // ** Tagger **
        //
        public void SetupTagger()
        {
            ClearValues();
            if (DataManager.IsRecentTagsPopulated())
            {
                repeatPreviousTags.IsEnabled = true;
            }
            DataManager.ClearGridList();
            refreshTags.IsEnabled = true;
            expansionControls.Visibility = Visibility.Visible;
            taggerTransition.Content = new TagExpander().Content;
        }
        public void SetupPages(List<Grid> grids)
        {
            foreach (var grid in grids)
            {
                GetGridList().Add(grid);
            }
        }
        private void ClearValues()
        {
            DataManager.SetTagList(new List<string>());
            taggerTransition.Content = new Grid();
            refreshTags.IsEnabled = false;
        }
        public void NextPage()
        {
            var content = GetGridList().First();
            taggerTransition.Content = content;
            GetGridList().Remove(content);
        }
        public void ViewTagResults()
        {
            taggerTransition.Content = (Grid)new resultsPage().Content;
        }

        // ** Search Box ** 
        //
        private void SetSearchBoxList()
        {
            if (searchString == searchBox.Text) return; // No need to update, the string is already filtered.

            searchString = searchBox.Text;

            if (SortByMissing.IsChecked)
            {
                AddSortConditions(TagSorting.SortByMissingTags);
            }
            if (SortByUnassigned.IsChecked)
            {
                AddSortConditions(TagSorting.SortByUnassignedFolder);
            }
            explorerList.ItemsSource = GetSortData(searchString);
        }
        // ** Misc. Functions ** 
        //
        public void SetupSelection(Button btn)
        {
            if ((bool)btn.Tag == false)
            {
                btn.Background = Brushes.DarkGray;
                btn.Tag = true;
                Keyboard.ClearFocus();
            }
            else
            {
                if ((bool)btn.Tag == true)
                {
                    btn.Tag = false;
                    btn.ClearValue(Control.BackgroundProperty);
                    Keyboard.ClearFocus();
                }
            }
        }
        public static MainWindow GetWindowObj()
        {
            return mainWindow;
        }
        public void ResetWindow()
        {
            // Write tags if it is not in sort mode.
            if (IOManager.GetCanSave())
            {
                if (!IsSortMode)
                {
                    IOManager.WriteTagsToObject();
                }
            }
            else
            {
                IOManager.ResetCanSave();
            }
            expansionControls.Visibility = Visibility.Hidden;
            taggerTransition.Content = new Grid();
        }
        public void EnableElement(UIElement element)
        {
            element.IsEnabled = true;
        }
        public void DisableElement(UIElement element)
        {
            element.IsEnabled = false;
        }
        public ListBox GetExplorerList()
        {
            return ExplorerList;
        }
        private void EnableAllElements()
        {
            searchBox.IsEnabled = true;
            mainMenu.IsEnabled = true;
        }
        private void DisableAllElements()
        {
            searchBox.IsEnabled = false;
            mainMenu.IsEnabled = false;
        }
        private void EnableMenuOptions()
        {
            SortByMissing.IsEnabled = true;
            SortByUnassigned.IsEnabled = true;
        }
        private void DisableMenuOptions()
        {
            SortByMissing.IsEnabled = false;
            SortByUnassigned.IsEnabled = false;

            SortByMissing.IsChecked = false;
            SortByUnassigned.IsChecked = false;
        }
        public static string GetCurrentSearch()
        {
            return searchString;
        }
        public void UpdateExplorerList(ObservableCollection<PenumbraObjectData> data)
        {
            ExplorerList.ItemsSource = data;
        }
        /*  ----------------------------- On Loaded -----------------------------
        */
        // ** Main Window ** 
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            Dispatcher.Invoke(async () =>
            {
                ObservableCollection<PenumbraObjectData> data = await LoadDataAsync();
                explorerList.ItemsSource = data;
                progressLoad.Visibility = Visibility.Hidden;
            });
            EnableAllElements();
        }
        // ** Explorer List ** 
        //
        private async void explorerList_Loaded(object sender, RoutedEventArgs e)
        {
            ExplorerList = sender as ListBox;
        }
        // ** Collections ** 
        //
        /*private void collectionLoaded(object sender, RoutedEventArgs e)
        {
            if (!collectionsTab.IsSelected)
                return;

            List<string> modStringData;
            if (collectionsGrid.Children.Count <= 0)
            {
                Dictionary<string, HashSet<string>> similarAuthors = new Dictionary<string, HashSet<string>>();
                modStringData = ButtonGrid.SetupCollectionsButton(sender as Grid, allItems);
                foreach (var val in modStringData)
                {
                    HashSet<string> similarArray = new HashSet<string>();
                    if (!similarAuthors.ContainsKey(val))
                    {
                        foreach (var valCompare in modStringData)
                        {
                            if (valCompare.Contains(val) && (!valCompare.Equals(val)))
                            {
                                similarArray.Add(valCompare);
                            }
                        }
                        if (similarArray.Count > 0)
                        {
                            similarAuthors.Add(val, similarArray);
                        }
                    }
                    if (!authorComboList.Items.Contains(val))
                    {
                        authorComboList.Items.Add(val);
                    }
                }
                Window window = new SimilarInfo(similarAuthors, allItems) { Owner = this };
                window.Show();
            }

         }*/
        /*  ----------------------------- On Closing -----------------------------
        */
        // ** Main Window ** 
        //
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = ConfigManager.GetPendingChanges();
        }
        // ** Menu Buttons ** 
        //
        private void ViewMissingTagCheck_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            if (IsMenuInteraction)
            {
                if (!IsSortMode)
                {

                    if (SortByMissing.IsEnabled == false)
                    {
                        EnableMenuOptions();
                        explorerList.ItemsSource = GetSortData();
                        return;
                    }
                    searchBox.Text = String.Empty;
                    searchString = String.Empty;

                    if (SortByMissing.IsChecked)
                    {
                        AddSortConditions(TagSorting.SortByMissingTags);
                    }
                    if (SortByUnassigned.IsChecked)
                    {
                        if (ConfigManager.GetPrimaryFolderSymbol().Equals("") || ConfigManager.GetNewModFolder().Equals(""))
                        {
                            MessageBox.Show("Configure primary symbol exclusion and new mod folder\nin File Menu to enable this sorting.");
                            SortByUnassigned.IsChecked = false;
                            SortByMissing.IsChecked = false;
                            DataManager.ClearSortConditions();
                            return;
                        }
                        AddSortConditions(TagSorting.SortByUnassignedFolder);
                    }
                    explorerList.ItemsSource = GetSortData();
                }
                else
                {
                    DisableMenuOptions();
                    explorerList.ItemsSource = GetSortData();
                }
            }
            IsMenuInteraction = false;
        }

        /* ----------------------------- Mouse Listeners -----------------------------
        */
        // ** Main Window ** 
        //
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (searchBox.IsFocused)
            {
                Keyboard.ClearFocus();
            }
        }
        // ** Explorer List ** 
        //
        private void explorerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (SortByModExists.IsChecked) return; // Tagger functionality is disabled if using Mod Exists Mode

            var dataSelection = GetSelectedItem(); // Current selected item
            var tagList = GetTagList();
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem == dataSelection) return; // Compare the new selection to the old one

            dataSelection = SetSelectedItem((PenumbraObjectData)lb.SelectedItem);

            if (dataSelection is not null)
            {
                SetupTagger();
                
                if (DataManager.GetRecentTagContent() is not null)
                {
                    repeatPreviousTags.IsEnabled = true;
                }
            }
        }
        private void explorerList_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            return; // Disabled
            //if (SortByModExists.IsChecked) return; // Tagger functionality is disabled if using Mod Exists Mode

            var dataSelection = GetSelectedItem();
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem == dataSelection) return;

            SetSelectedItem((PenumbraObjectData)lb.SelectedItem);

            GetModMetaData().TryGetValue(GetSelectedItem().objectName, out var value);
            ModInfoWindow window = new ModInfoWindow(value) { Owner = this };
            window.Show();
        }
        // ** Menu Buttons ** 
        //
        private void repeatPreviousTags_Click(object sender, RoutedEventArgs e)
        {
            // Instantly send to results page. The tagList is already defined from the previous interaction.
            // NOTE: At least one tag must be done before this is available!
            //taggerTransition.Content = (Grid)new resultsPage(true).Content;
            taggerTransition.Content = DataManager.GetRecentTagContent();
        }
        // ** Search Box ** 
        //
        private void searchBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            SetSearchBoxList();
        }
        private void searchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetSearchBoxList();
            }
        }
        // ** Collections ** 
        //
        private void collectionsTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainMenu.IsEnabled = false;
            explorerList.Visibility = Visibility.Hidden;
            fileExplorerLabel.Visibility = Visibility.Hidden;
            searchBox.Visibility = Visibility.Hidden;

        }
        private void Collections_SortBySelection_Changed(object sender, SelectionChangedEventArgs e)
        {
        }
        private void Collections_AuthorSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
        }
        // ** Tabs ** 
        //
        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*mainMenu.IsEnabled = true;
            explorerList.Visibility = Visibility.Visible;
            fileExplorerLabel.Visibility = Visibility.Visible;
            searchBox.Visibility = Visibility.Visible;
            fileDirButton.Visibility = Visibility.Visible;
            dirTextBox.Visibility = Visibility.Visible;*/
        }
        // ** Staging Label Dir **
        private void fileDirSet_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            folder.ShowDialog();
            
            ConfigManager.SetStagingDirectory(folder.SelectedPath);
        }

        private void MenuButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMenuInteraction = true;
        }

        private void ModPropertiesBtn_Click(object sender, MouseButtonEventArgs e)
        {
            Window window = new ModProperties() { Owner = this };
            window.Show();
        }

        private void openSortFolder_Click(object sender, RoutedEventArgs e)
        {
            FileManager.OpenFolder(Folders.SortFolder);
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to save " + IOManager.GetPendingSaves() + " changes?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                IOManager.WriteDataToSort();
                saveAll.IsEnabled = false;
            }
        }
        private void refresh_TagsClick(object sender, RoutedEventArgs e)
        {
            DataManager.UpdatePageTags();
        }
        private void primarySymbol_Click(object sender, RoutedEventArgs e)
        {
            InputWindow dialog = new InputWindow("PrimarySymbol") { Owner = this };
            dialog.ShowDialog();
            ConfigManager.SetPrimaryFolderSymbol(InputWindow.returnVal);
            ConfigManager.WriteConfigValue("primarySymbol", InputWindow.returnVal);
            ConfigManager.SetConfiguration();

        }
        private void newFolder_Click(object sender, RoutedEventArgs e)
        {
            InputWindow dialog = new InputWindow("NewFolder") { Owner = this };
            dialog.ShowDialog();
            ConfigManager.SetNewModFolder(InputWindow.returnVal);
            ConfigManager.WriteConfigValue("newFolder", InputWindow.returnVal);
            ConfigManager.SetConfiguration();
        }
        private void expander_Click(object sender, RoutedEventArgs e)
        {
            taggerTransition.Content = new TagExpander().Content;
        }

        private void expansionControls_expandClick(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)taggerTransition.Content;

            var grid = scrollViewer.Content as Grid;

            var stackPanel = grid.Children[0] as StackPanel;

            foreach (var child in stackPanel.Children)
            {
                var expander = child as Expander;
                expander.IsExpanded = true;
            }
            
        }
        private void expansionControls_collapseClick(object sender, RoutedEventArgs e)
        {
            foreach (var child in GetExpanders().Children)
            {
                var expander = child as Expander;
                expander.IsExpanded = false;
            }
        }
        private void finishBtn(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                DataManager.SetRecentTagContent((ScrollViewer)taggerTransition.Content);
                foreach (Expander el in GetExpanders().Children)
                {
                    if (el.Header.Equals("presets"))
                        continue;

                    StackPanel stackPanel = (StackPanel)el.Content;

                    Grid grid = (Grid)stackPanel.Children[0];

                    var tl = DataManager.GetTagList();
                    foreach (Button btn in grid.Children)
                    {
                        if ((bool)btn.Tag == true)
                        {
                            if (!tl.Contains(el.Header.ToString().ToLower()))
                            {
                                tl.Add(el.Header.ToString().ToLower());
                            }
                            tl.Add(btn.Content.ToString().ToLower());
                        }
                    }
                }
                if (DataManager.GetTagList().Count <= 0)
                {
                    // Sort Mode, grab existing tags to create folders!
                    IsSortMode = true;
                    SetTagList(GetSelectedItem().jsonInfo.LocalTags.Select(s => (string)s).ToList());
                }
                ViewTagResults();
                expansionControls.Visibility = Visibility.Hidden;
            }
        }
        private StackPanel GetExpanders()
        {
            ScrollViewer scrollViewer = (ScrollViewer)taggerTransition.Content;

            var grid = scrollViewer.Content as Grid;

            return grid.Children[0] as StackPanel;
        }
    }
}
