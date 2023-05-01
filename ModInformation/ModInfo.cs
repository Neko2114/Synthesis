using Synthesis.Managers;
using System.IO;
using System.Linq;

namespace Synthesis.ModInformation
{
    public class ModInfo
    {
        private bool InMetaData;
        private bool InSortData;
        private bool InModFolder;
        private bool InModData;
        private string modName;

        public ModInfo(string name)
        {
            modName = name;
            InMetaData = IsInMeta();
            InSortData = IsInSort();
            InModFolder = IsInModFolder();
            InModData = IsInModData();
        }
        private bool IsInMeta()
        {
            return DataManager.GetModMetaData().ContainsKey(modName);
        }
        private bool IsInSort()
        {
            return DataManager.GetSortListData().Data.ContainsKey(modName);
        }
        private bool IsInModFolder()
        {
            return Directory.GetDirectories(ConfigManager.GetStagingDirectory()).Any(x => x.Contains(modName));
        }
        private bool IsInModData()
        {
            return DataManager.GetPenumbraData().Any(x => x.objectName.Equals(modName));
        }
        public bool ExistsInMeta()
        {
            return InMetaData;
        }
        public bool ExistsInSortData()
        {
            return InSortData;
        }
        public bool ExistsInModFolder()
        {
            return InModFolder;
        }
        public bool ExistsInModData()
        {
            return InModData;
        }
        public string GetModName()
        {
            return modName;
        }
        public bool IsScrapped()
        {
            return ExistsInModData() == true && (ExistsInMeta() == false && ExistsInSortData() == false && ExistsInModFolder() == false);
        }
    }
}

// To successfully rename mod folders, the following needs to be done:
// -- Rename the folder in FFXIVM
// -- Rename the meta.json "name" field in the FFXIVM folder
// -- Rename the temporary data mod_data .json folder
// -- Rename the string relationship for the mod in sort_order.json