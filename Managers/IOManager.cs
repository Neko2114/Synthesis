using Newtonsoft.Json;
using Synthesis.Penumbra;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Synthesis.Managers
{
    public class IOManager
    {
        private static int saveCount = 0;
        private static bool CanSave = true;

        /* ----------------------- Saving ----------------------- 
         */

        public static void SaveSortPathLocally(string path, string selectedName)
        {
            var window = MainWindow.GetWindowObj();
            var sortList = DataManager.GetSortListData();

            if (!sortList.Data.ContainsKey(selectedName))
            {
                MessageBox.Show("Key Not Found!\n" + selectedName);
                CanSave = false;
                window.ResetWindow();
                return;
            }

            if (!DataManager.GetAvailableFolders().Any(x => x.Equals(path)))
            {
                MessageBox.Show("Invalid folder path:\n" + path);
                CanSave = false;
                window.ResetWindow();
                return;
            }


            var sortIndex = sortList.Data[selectedName];

            if (!sortList.Data.TryGetValue(selectedName, out var name))
                return;

            name = name.Substring(sortIndex.LastIndexOf('/') + 1);
            sortList.Data[selectedName] = path + name;
            saveCount++;
            window.saveAll.IsEnabled = true;
            //var test = DataManager.GetTagList();

            window.ResetWindow();
        }
        /* ----------------------- Writing ----------------------- 
        */

        public static void WriteTagsToObject()
        {
            var selectedItem = DataManager.GetSelectedItem();
            selectedItem.jsonInfo.LocalTags = DataManager.GetTagList().ToList<object>();
            selectedItem.hasTags = true;

            var directory = ConfigManager.GetPenumbraDirectory();
            var directoryBak = directory + "\\bak\\";

            if (!Directory.Exists(directoryBak)) Directory.CreateDirectory(directoryBak);

            var _data = JsonConvert.SerializeObject(selectedItem.jsonInfo, Formatting.Indented);

            if (!File.Exists(directoryBak + selectedItem.fileInfo.Name))
            {
                File.Copy(directory + selectedItem.fileInfo.Name, directoryBak + selectedItem.fileInfo.Name); // Copy => mod_data\(mod_name).json => mod_data\bak\(mod_name).json
            }

            File.WriteAllText(directory + selectedItem.fileInfo.Name, _data); // Write => mod_data\(mod_name).json

            Logger.WriteToLog("Wrote data to " + directory + ": " + selectedItem.objectName);
        }
        public static void WriteDataToSort()
        {
            var fileNameBak = "sort_order.bak";
            var selectedItem = DataManager.GetSelectedItem();
            var directoryBak = selectedItem.fileInfo.Directory + "\\bak\\";

            var _data = JsonConvert.SerializeObject(DataManager.GetSortListData(), Formatting.Indented);

            if (!Directory.Exists(directoryBak)) Directory.CreateDirectory(directoryBak);
            if (!Directory.Exists(ConfigManager.GetSortOrderDirectory() + "output\\")) Directory.CreateDirectory(ConfigManager.GetSortOrderDirectory() + "output\\");

            if (!File.Exists(directoryBak + fileNameBak))
            {
                File.Copy(ConfigManager.GetSortOrderDirectory() + "sort_order.json", directoryBak + fileNameBak); // Copy => sort_order.json => bak\sort_order.json
            }
            var fileName = GenerateFileName("sort_order") + ".json";
            File.WriteAllText(ConfigManager.GetSortOrderDirectory() + "output\\" + fileName, _data); // Write => output\sort_order(random).json
            File.WriteAllText(ConfigManager.GetSortOrderDirectory() + "sort_order.json", _data); // Write => output\sort_order.json

            Logger.WriteToLog("\nWrote data to " + ConfigManager.GetSortOrderDirectory() + "output\\" + fileName + selectedItem.objectName + "\n");

            saveCount = 0;
        }
        public static int GetPendingSaves()
        {
            return saveCount;
        }
        public static bool GetCanSave()
        {
            return CanSave;
        }
        public static void ResetCanSave()
        {
            CanSave = true;
        }
        private static string GenerateFileName(string context)
        {
            return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        }
    }

}
