using Newtonsoft.Json;
using Synthesis.Pages;
using Synthesis.Penumbra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Penumbra_.Localization.Localization;

namespace Synthesis.Managers
{
    public class DataManager
    {

        // Data
        //
        private static ObservableCollection<PenumbraObjectData> allItems;
        private static ObservableCollection<PenumbraObjectData> sortedItems;
        private static HashSet<string> availableFolders = new HashSet<string>();
        private static List<string> tagList;
        private static List<Grid> gridList;
        private static Dictionary<string, PenumbraMetaData> modMeta;
        private static PenumbraSortOrder sortList;
        //public Dictionary<string, string> sortConversion = new Dictionary<string, string>();
        private static PenumbraObjectData selectedItem;
        private static PageTags pageData;
        private static List<TagSorting> sortConditions;
        private static MainWindow mainWindow;
        private static List<Preset> presetData;
        private static List<string> lastTagList;
        private static ScrollViewer lastTagContent;

        // Folders
        //

        public enum TagSorting
        {
            SortByMissingTags = 0,
            SortByUnassignedFolder = 1,
            SortByInvalidTags = 2,
            SortByTagged = 3,
            SortByModExists = 4
        }
        public enum Folders
        {
            SortFolder = 0
        }

        public DataManager()
        {
            allItems = new ObservableCollection<PenumbraObjectData>();
            sortedItems = new ObservableCollection<PenumbraObjectData>();
            availableFolders = new HashSet<string>();
            tagList = new List<string>();
            lastTagList = new List<string>();
            gridList = new List<Grid>();
            modMeta = new Dictionary<string, PenumbraMetaData>();
            sortList = new PenumbraSortOrder();
            //sortConversion = new Dictionary<string, string>();
            selectedItem = null;
            pageData = new PageTags();
            sortConditions = new List<TagSorting>();
            presetData = new List<Preset>();
            mainWindow = MainWindow.GetWindowObj();
        }
        public static void Init()
        {
            new ConfigManager();
            new IOManager();
            new DataManager();
        }
        private static ObservableCollection<PenumbraObjectData> Load()
        {
            DirectoryInfo PenumbraDirectory = new DirectoryInfo(ConfigManager.GetPenumbraDirectory());
            FileInfo[] files = PenumbraDirectory.GetFiles("*.json");

            // ModMeta Data
            //
            modMeta = new Dictionary<string, PenumbraMetaData>();

            var modMetaDir = ConfigManager.GetMetaDirectory();
            if (modMetaDir is not null) // Checks if staging path is set before enabling this feature.
            {
                var dimc = new DirectoryInfo(modMetaDir);
                foreach (var dir in dimc.GetDirectories())
                {
                    if (File.Exists(dir.FullName + "\\" + "meta.json"))
                    {
                        modMeta.Add(dir.Name, JsonConvert.DeserializeObject<PenumbraMetaData>(File.ReadAllText(dir.FullName + "\\" + "meta.json")));
                    }

                }
            }

            // AllFiles Data
            //
            foreach (var fileInfo in files)
            {
                allItems.Add(new PenumbraObjectData(fileInfo, JsonConvert.DeserializeObject<PenumbraModData>(File.ReadAllText(fileInfo.FullName))));
            }
            // SortList Data
            //
            sortList = JsonConvert.DeserializeObject<PenumbraSortOrder>(File.ReadAllText(ConfigManager.GetSortOrderDirectory() + "sort_order.json"));

            // Avaiable Folders
            //
            foreach (var folderPath in sortList.Data.Values)
            {
                var count = folderPath.Count(x => x == '/');

                if (count <= 0) continue;

                foreach (var index in AllIndexOf(folderPath, '/'))
                {
                    availableFolders.Add(folderPath.Substring(0, index) + '/');
                }

            }
            foreach (var emptyFolder in sortList.EmptyFolders)
            {
                availableFolders.Add(emptyFolder + '/');
            }
            // PageTag Data
            //
            if (!File.Exists("page_tags.json"))
                File.WriteAllText("page_tags.json", JsonConvert.SerializeObject(new PageTags()
                {
                    head = new string[] { "example" },
                    body = new string[] { "example" },
                    legs = new string[] { "example" },
                    shoes = new string[] { "example" },
                    accessories = new string[] { "example" },
                    animation = new string[] { "example" },
                    bdsm = new string[] { "example" },
                    hair = new string[] { "example" },
                    piercing = new string[] { "example" },
                    hands = new string[] { "example" },
                    swimsuit = new string[] { "example" },
                    extras = new string[] { "example" },
                    options = new string[] { "example" },
                    presets = new string[][] {
                        new string[] {
                            "Dress",
                            "! Upper Wear/Dress/",
                            "dress"
                        }
                    }
                }, Formatting.Indented));

            pageData = JsonConvert.DeserializeObject<PageTags>(File.ReadAllText("page_tags.json"));


            foreach (var page in pageData.GetType().GetProperties())
            {
                if (!page.Name.Equals("presets"))
                {
                    var sort = (string[])page.GetValue(pageData, null);
                    page.SetValue(pageData, sort.OrderBy(x => x).Cast<string>().ToArray());
                }
                else
                {
                    foreach(var preset in pageData.presets)
                    {
                        presetData.Add(new Preset(preset[0], preset[1], preset.Skip(2).ToArray()));
                    }
                }
            }

            return allItems;
        }
        public static async Task<ObservableCollection<PenumbraObjectData>> LoadDataAsync()
        {
            mainWindow.progressLoad.Visibility = Visibility.Visible;
            var data = await Task.Run(() => Load());
            return data;

        }
        public PenumbraObjectData GetSortedList(string c)
        {
            foreach (var data in allItems)
            {
                var stringData = data.objectName;
                if (stringData.ToLower().Contains(c.ToLower()))
                {
                    return data;
                }
            }
            return null;
        }

        public static void AddSortConditions(TagSorting e)
        {
            sortConditions.Add(e);
        }
        public static void ClearSortConditions()
        {
            sortConditions.Clear();
        }
        public static ObservableCollection<PenumbraObjectData> GetSortData(string filter = "")
        {
            var list = allItems.ToList();
            if (sortConditions.Count > 0)
            {
                if (!filter.Equals(""))
                {
                    list = list.Where(s => s.objectName.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();
                }
                foreach (var cond in sortConditions)
                {
                    if (cond == TagSorting.SortByMissingTags)
                    {
                        if (sortConditions.Contains(TagSorting.SortByTagged))
                        {
                            var window = MainWindow.GetWindowObj();
                            sortConditions.Remove(TagSorting.SortByTagged);

                            window.SortByMissing.IsChecked = false;
                            //window.SortByTagged.IsChecked = false;
                            continue; // Skips sorting for both missing & tagged if both are enabled at the same time.
                        }
                        list = list.Where(s => s.jsonInfo.LocalTags.Count == 0).ToList();
                    }
                    if (cond == TagSorting.SortByUnassignedFolder)
                    {
                        list = list.Where(s => sortList.Data.TryGetValue(s.objectName, out var _))
                                                                                       .Where(s => !sortList.Data[s.objectName].Contains(ConfigManager.GetPrimaryFolderSymbol())
                                                                                       || sortList.Data[s.objectName].Contains(ConfigManager.GetNewModFolder())).ToList();
                    }
                    if (cond == TagSorting.SortByInvalidTags)
                    {
                        // TODO

                        /*sortedItems = new ObservableCollection<PenumbraObjectData>();

                        var tagItems = allItems.Where(s => s.jsonInfo.LocalTags.Count > 0);
                        foreach (var item in tagItems)
                        {
                            var totalInvalidTags = 0;
                            foreach (var tag in item.jsonInfo.LocalTags)
                            {
                                var broke = false;
                                foreach (var folder in availableFolders)
                                {
                                    if (folder.ToLower().Contains((string)tag))
                                    {
                                        totalInvalidTags = 0;
                                        broke = true;
                                        break;
                                    }
                                    else
                                    {
                                        // This means that a tag was found that cannot resolve to a folder.
                                        totalInvalidTags++;
                                    }
                                }
                                if (broke)
                                {
                                    break;
                                }
                            }
                            if (totalInvalidTags > 0)
                            {
                                sortedItems.Add(item);
                            }
                        }*/
                        if (cond == TagSorting.SortByTagged)
                        {
                            list = list.Where(s => s.jsonInfo.LocalTags.Count > 0).ToList();
                        }
                    }
                }
            }
            else
            {
                return new ObservableCollection<PenumbraObjectData>(allItems.Where(s => s.objectName.Contains(MainWindow.GetCurrentSearch(), StringComparison.CurrentCultureIgnoreCase)));
            }
            sortConditions.Clear();
            return new ObservableCollection<PenumbraObjectData>(list);
        }

        public static Dictionary<string, PenumbraMetaData> GetModMetaData()
        {
            return modMeta;
        }
        public static PenumbraSortOrder GetSortListData()
        {
            return sortList;
        }
        public static PenumbraObjectData SetSelectedItem(PenumbraObjectData obj)
        {
            selectedItem = obj;
            return selectedItem;
        }
        public static PenumbraObjectData GetSelectedItem()
        {
            if (selectedItem is not null)
                return selectedItem;
            else
                //MessageBox.Show("SelectedItem returned null!");
                return null;
        }
        public static string GetModVersion(string objectName)
        {
            if (modMeta.TryGetValue(objectName, out var modInfo))
            {
                if (modInfo.Version.Equals(""))
                {
                    return GetString(StringErrors.MetaNotFound);
                }
                return modInfo.Version;
            }
            return GetString(StringErrors.MetaNotFound);
        }

        public static string GetModDescription(string objectName)
        {
            if (modMeta.TryGetValue(objectName, out var modInfo))
            {
                if (modInfo.Description.Equals(""))
                {
                    return GetString(StringErrors.MetaNotFound);
                }
                return modInfo.Description;
            }
            return GetString(StringErrors.MetaNotFound);
        }

        public static string GetModAuthor(string objectName)
        {
            if (modMeta.TryGetValue(objectName, out var modInfo))
            {
                if (modInfo.Author.Equals(""))
                {
                    return GetString(StringErrors.MetaNotFound);
                }
                return modInfo.Author;
            }
            return GetString(StringErrors.MetaNotFound);
        }
        public static void SetTagList(List<string> list)
        {
            if (list.Count == 0)
                SetRecentTags();

            tagList = list;
        }

        public static List<string> GetTagList()
        {
            return tagList;
        }

        public static HashSet<string> GetAvailableFolders()
        {
            return availableFolders;
        }
        public static string[] GetPageData(string title, int arrayIndex = 0)
        {
            var propInfo = pageData.GetType().GetProperty(title);
            return (string[])propInfo.GetValue(pageData, null);
        }
        public static string[] GetPageDataElements()
        {
            return pageData.GetType().GetProperties().Select(x => x.Name).ToArray();
        }
        public static int GetPageDataSize(string title, int arrayIndex = 0)
        {
            string[] propInfo = null;
            propInfo = (string[])pageData.GetType().GetProperty(title).GetValue(pageData, null);
            return propInfo.Count();
        }
        public static string[][] GetPresetData(string title, int arrayIndex = 0)
        {
            return pageData.presets;
        }
        public static int GetPresetDataSize(string title, int arrayIndex = 0)
        {
            return pageData.presets.Count();
        }
        public static Preset GetPresetButtonData(string name)
        {
            Preset data;
            try
            {
                data = presetData.Single(x => x.GetName().Equals(name));
            }
            catch
            {
                MessageBox.Show("Unable to find data for value " + name);
                throw;
            }
            return data;
        }
        public static List<Grid> GetGridList()
        {
            return gridList;
        }
        public static void ClearGridList()
        {
            gridList.Clear();
        }
        public void SetGridList(List<Grid> grid)
        {
            gridList = grid;
        }
        public static ObservableCollection<PenumbraObjectData> GetPenumbraData()
        {
            return allItems;
        }
        public static IList<int> AllIndexOf(string text, char str)
        {
            IList<int> allIndexOf = new List<int>();
            int index = text.IndexOf(str);
            while (index != -1)
            {
                allIndexOf.Add(index);
                index = text.IndexOf(str, index + 1);
            }
            return allIndexOf;
        }

        public static List<string> GetMatchingPaths(string tag)
        {
            List<string> pathList = new List<string>();
            foreach (var path in DataManager.GetAvailableFolders())
            {
                if (path.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pathList.Add(path);
                }
            }
            return pathList;
        }
        public static void UpdatePageTags()
        {
            pageData = JsonConvert.DeserializeObject<PageTags>(File.ReadAllText("page_tags.json"));
            presetData = new List<Preset>();

            foreach (var page in pageData.GetType().GetProperties())
            {
                if (!page.Name.Equals("presets"))
                {
                    var sort = (string[])page.GetValue(pageData, null);
                    page.SetValue(pageData, sort.OrderBy(x => x).Cast<string>().ToArray());
                }
                else
                {
                    foreach (var preset in pageData.presets)
                    {
                        presetData.Add(new Preset(preset[0], preset[1], preset.Skip(2).ToArray()));
                    }
                }
            }
            MessageBox.Show("Updated values from page_data.json!");
        }
        public static void SetRecentTags()
        {
            lastTagList = tagList;
        }
        public static List<string> GetRecentTags()
        {
            return lastTagList;
        }
        public static bool IsRecentTagsPopulated()
        {
            return lastTagList.Count > 0;
        }
        public static void SetRecentTagContent(ScrollViewer sv)
        {
            lastTagContent = sv;
        }
        public static ScrollViewer GetRecentTagContent()
        {
            return lastTagContent;
        }
        /*private ObservableCollection<ModObject> GetFilteredStringList(string filter)
{
// Mod exists.

searchString = filter;
var sortJsonResults = new ObservableCollection<ModObject>();

var existanceResults = sortList.Data.Keys.Where(s => s.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();
existanceResults.AddRange(stagingItems.Where(x => x.Contains(searchBox.Text, StringComparison.CurrentCultureIgnoreCase)).ToList());
if (stagingItems.Count() > 0 && dirTextBox.Text != null && dirTextBox.IsEnabled == false)
{
// Append search results from staging grounds folder. This is optional.
foreach (var item in existanceResults)
{
sortJsonResults.Add(new ModObject(Path.GetFileNameWithoutExtension(item)));
}
}

return sortJsonResults;
}*/
        /* private void SetupFileObjects()
        {
           
        }*/
        /*public Dictionary<string, string> GetConversionData()
        {
            // TODO
            foreach (var item in sortList.Data)
            {
                sortConversion.Add(item.Key, item.Value.Substring(item.Value.LastIndexOf('/') + 1));
            }
            return sortConversion;
        }*/
    }
}/*
public class ModObject
{
    public string objectName { get; set; }
    public FileInfo fileInfo { get; set; }

    public ModObject(string name)
    {
        objectName = name;
    }
}*/