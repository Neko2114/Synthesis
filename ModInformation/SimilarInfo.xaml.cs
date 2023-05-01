using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Penumbra_.Localization;

namespace Penumbra_
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SimilarInfo : MetroWindow
    {
        private static MainWindow window;
        private static ObservableCollection<MainWindow.PenumbraObject> allItems;
        public SimilarInfo(Dictionary<string, HashSet<string>> similarities, ObservableCollection<MainWindow.PenumbraObject> allitems)
        {
            InitializeComponent();
            window = (MainWindow)Application.Current.MainWindow;

            allItems = allitems;

            foreach (var info in similarities)
            {
                if (info.Value.Contains("TexTools U"))
                {
                    var test123 = 1;
                }
                Expander expander = new Expander()
                {
                    Header = info.Key,
                    Content = string.Join("\n", info.Value.ToArray())
                };
                expander.MouseDoubleClick += expander_DoubleClick;
                similarInfoStackPanel.Children.Add(expander);
            }
        }
        private void expander_DoubleClick(object sender, RoutedEventArgs e)
        {
            // To successfully rename mod folders, the following needs to be done:
            // -- Rename the folder in FFXIVM
            // -- Rename the meta.json "name" field in the FFXIVM folder
            // -- Rename the temporary data mod_data .json folder
            // -- Rename the string relationship for the mod in sort_order.json

            var expander = sender as Expander;
            MessageBoxResult confirmation = MessageBoxResult.None;
            MessageBoxResult confirmationLoop = MessageBoxResult.None;
            List<string> invalidDirectories = new List<string>();
            List<string> duplicateDirectories;

            if (expander.IsExpanded)
            {
                MessageBoxResult result = MessageBox.Show(GetString(StringMessages.ReplaceSimilarAuthors), GetString(StringMessages.ReplaceSimilarAuthorsHeader), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var replaceString = expander.Content.ToString().Split('\n');
                    var metaData = window.GetModMetaData();
                    List<MainWindow.ModMeta> changes = new List<MainWindow.ModMeta>();

                    foreach (var replace in replaceString)
                    {
                        changes.AddRange(metaData.Values.Where(x => x.Author.Contains(replace)));
                    }
                    if (changes.Count() == 0)
                    {
                        similarInfoStackPanel.Children.Remove(expander);
                        return;
                    }

                    foreach (var change in changes)
                    {
                        change.Author = expander.Header.ToString();
                    }

                    var folderName = string.Empty;
                    var folderNameBak = string.Empty;

                    var directory = string.Empty;
                    var directoryBak = string.Empty;

                    var changesConfirmed = 0;
                    bool processAllChanges = false; 
                    
                    if (changes.All(str => replaceString.First().Contains(str.Author) && replaceString.Count() == 1))
                    {
                        processAllChanges = true;
                    }

                    if (processAllChanges)
                    {
                        confirmationLoop = MessageBox.Show("Are you sure you want to overwrite " + changes.Count() + " changes in meta.json? \nAuthor: " + replaceString.First(), "Confirmation",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                        foreach (var change in changes)
                        {
                            var nameObj = window.sortConversion.Keys.Where(x => change.Name.Trim().Contains(x));
                            var test = nameObj.Count();
                            foreach (var nameItemLoop in nameObj)
                            {
                                MainWindow.PenumbraObject itemsLoop = window.GetSortListData(nameItemLoop);

                                if (!itemsLoop.objectName.Equals(change.Name.Trim()))
                                {
                                    duplicateDirectories = new List<string>();
                                    duplicateDirectories.Add(change.Name);
                                    File.AppendAllLines("duplicate_directories.txt", duplicateDirectories);
                                    continue;
                                }

                                folderName = itemsLoop.objectName + "\\";
                                folderNameBak = folderName + "bak\\";

                                directory = "D:\\FFXIVM\\" + folderName;
                                directoryBak = directory + "bak\\";

                                if (!Directory.Exists(directory))
                                {
                                    invalidDirectories.Add(directory);
                                    File.AppendAllLines("invalid_directories.txt", invalidDirectories);
                                    continue;
                                }

                                if (!Directory.Exists(directoryBak)) Directory.CreateDirectory(directoryBak);
                                var _dataLoop = JsonConvert.SerializeObject(change, Formatting.Indented);


                                if (!File.Exists(directoryBak + "meta.json"))
                                {
                                    File.Copy(directory + "meta.json", directoryBak + "meta.json"); // Creates a copy of the file before writing it.
                                }

                                File.WriteAllText(directory + "meta.json", _dataLoop);
                                changesConfirmed++;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in changes)
                        {
                            var nameObj = window.sortConversion.Keys.Where(x => item.Name.Trim().Contains(x));

                            if (nameObj.Count() is 0)
                                throw new System.Exception();


                            foreach (var nameItem in nameObj)
                            {

                                MainWindow.PenumbraObject items = window.GetSortListData(nameItem);


                                if (!items.objectName.Equals(item.Name.Trim()))
                                {
                                    duplicateDirectories = new List<string>();
                                    duplicateDirectories.Add(item.Name.Trim());
                                    File.AppendAllLines("duplicate_directories.txt", duplicateDirectories);
                                    continue;
                                }

                                folderName = items.objectName + "\\";
                                folderNameBak = folderName + "bak\\";

                                directory = "D:\\FFXIVM\\" + folderName;
                                directoryBak = directory + "bak\\";

                                if (!Directory.Exists(directory))
                                {
                                    invalidDirectories.Add(directory);
                                    File.AppendAllLines("invalid_directories.txt", invalidDirectories);
                                    return;
                                }

                                if (!Directory.Exists(directoryBak)) Directory.CreateDirectory(directoryBak);
                                var _data = JsonConvert.SerializeObject(item, Formatting.Indented);

                                confirmation = MessageBox.Show("Are you sure you want to overwrite the meta.json?\n Folder: " + items.objectName + ", Author: " + items.author, "Confirmation",
                                                     MessageBoxButton.YesNoCancel,
                                                     MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    if (!File.Exists(directoryBak + "meta.json"))
                                    {
                                        File.Copy(directory + "meta.json", directoryBak + "meta.json"); // Creates a copy of the file before writing it.
                                    }

                                    File.WriteAllText(directory + "meta.json", _data);
                                    changesConfirmed++;
                                }
                                else if (confirmation == MessageBoxResult.No)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    if (changesConfirmed == changes.Count())
                    {
                        similarInfoStackPanel.Children.Remove(expander);
                    }
                }
                if (confirmationLoop == MessageBoxResult.Cancel)
                {
                    MessageBox.Show("Operation cancelled.");
                }
            }
            else
            {
                return;
            }
        }
    }
}
