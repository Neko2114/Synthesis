using Synthesis.ModInformation;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Synthesis.Managers.DataManager;

namespace Synthesis.Managers
{
    public class FileManager
    {
        public static bool Contains()
        {
            return false;
        }

        // To successfully rename mod folders, the following needs to be done:
        // -- Rename the folder in FFXIVM
        // -- Rename the meta.json "name" field in the FFXIVM folder
        // -- Rename the temporary data mod_data .json folder
        // -- Rename the string relationship for the mod in sort_order.json

        public static Dictionary<List<char>, string> GetBadFileNames()
        {
            // allData objectName === mod_data json file name
            // meta => Key === allData objectName, Value "Name" contains mod name
            // sortList (possible) Key === allData objectName

            var files2 = ConfigManager.GetStagingDirectory();
            var files = DataManager.GetPenumbraData();
            var sort = DataManager.GetSortData();
            var sortList = DataManager.GetSortListData();
            var meta = DataManager.GetModMetaData();

            var badStrings = new Dictionary<List<char>, string>();
            foreach (var file in files)
            {

                //var path = Path.GetFileNameWithoutExtension(file);
                var chars = file.objectName.Where(x => !char.IsAscii(x)).ToList();
                if (chars.Count > 0)
                {
                    badStrings.Add(chars, file.objectName);
                }
            }
            /*foreach (var file in files)
            {
                var path = Path.GetFileNameWithoutExtension(file);
                var chars = path.Where(x => !char.IsAscii(x)).ToList();
                if (chars.Count > 0)
                {
                    badStrings.Add(chars, path);
                }
            }*/
            //var test = files.Where(x => !x.All(char.IsLetterOrDigit)).SelectMany(x => char.IsLetterOrDigit(x));
            /*
            foreach (var file in ))
            {
                file.SelectMany(b => b).ToList();
            }*/
            return badStrings;
        }
        public static Dictionary<string, string> GetDuplicateMods()
        {
            var files = DataManager.GetPenumbraData();
            var sortList = DataManager.GetSortListData();
            var dupeList = new Dictionary<string, string>();
            foreach (var file in files)
            {
                if (file.objectName.Contains("(2)"))
                {
                    var str = file.objectName.Substring(0, file.objectName.LastIndexOf("(2)") - 1).Trim();
                    if (sortList.Data.ContainsKey(str))
                    {
                        dupeList.Add(file.objectName, str);
                    }
                }
            }
            return dupeList;
        }
        public static List<ModInfo> GetDeletedScraps()
        {
            var files = GetPenumbraData();
            List<ModInfo> scrap = new List<ModInfo>();
            foreach (var file in files)
            {
                var modInfo = new ModInfo(file.objectName);
                if (modInfo.IsScrapped())
                {
                    scrap.Add(modInfo);
                }
            }
            return scrap;
        }
        public static void OpenFolder(Folders folder)
        {
            if (folder == Folders.SortFolder)
            {
                var folderPath = ConfigManager.GetSortOrderDirectory();
                if (Directory.Exists(folderPath))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = folderPath,
                        FileName = "explorer.exe"

                    };
                    Process.Start(startInfo);
                }
            }
        }
    }
}
