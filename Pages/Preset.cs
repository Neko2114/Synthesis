using Newtonsoft.Json;

namespace Synthesis.Pages
{
    public class Preset
    {
        private string name;
        private string dir;
        private string[] tags;

        public Preset(string presetName, string dirName, string[] tagList)
        {
            name = presetName;
            dir = dirName;
            tags = tagList;
        }
        public string GetName()
        {
            return name;
        }
        public string GetDir()
        {
            return dir;
        }
        public string[] GetTags()
        {
            return tags;
        }
    }
}
