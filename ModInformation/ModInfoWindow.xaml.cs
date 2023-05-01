using MahApps.Metro.Controls;
using Synthesis.Penumbra;

namespace Synthesis
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ModInfoWindow : MetroWindow
    {
        public ModInfoWindow(PenumbraMetaData modInfo)
        {
            InitializeComponent();
            nameLabel.Content = "Name: " + modInfo.Name;
            versionLabel.Content = "Version: " + modInfo.Version;
            authorLabel.Content = "Author: " + modInfo.Author;
            descLabel.Text = modInfo.Description;
        }
    }
}
