using Synthesis.Managers;
using System.IO;

namespace Synthesis.Penumbra
{
    public class PenumbraObjectData
    {
        public string objectName { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public bool hasTags { get; set; }
        public FileInfo fileInfo { get; set; }
        public PenumbraModData jsonInfo { get; set; }

        public PenumbraObjectData(FileInfo file, PenumbraModData data)
        {
            objectName = file.Name.Split(".json")[0];
            fileInfo = file;
            jsonInfo = data;
            author = DataManager.GetModAuthor(objectName);
            description = DataManager.GetModDescription(objectName);
            version = DataManager.GetModVersion(objectName);

        }
    }
}
