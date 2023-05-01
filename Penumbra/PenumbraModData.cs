using System.Collections.Generic;

namespace Synthesis.Penumbra
{
    public class PenumbraModData
    {
        public int FileVersion { get; set; }
        public long ImportDate { get; set; }
        public List<object> LocalTags { get; set; }
        public string Note { get; set; }
        public bool Favorite { get; set; }
    }
}
