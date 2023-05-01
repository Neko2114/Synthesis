using MahApps.Metro.Controls;
using Synthesis.Managers;
using Synthesis.ModInformation;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ModProperties : MetroWindow
    {
        public ModProperties()
        {
            InitializeComponent();

            // Duplicate Mods
            Expander dupeExpander = new Expander()
            {
                Header = "Duplicate Mods",
                Content = new StackPanel()
            };
            similarInfoStackPanel.Children.Add(dupeExpander);

            var dupes = FileManager.GetDuplicateMods();
            StackPanel dupePanel = dupeExpander.Content as StackPanel;
            foreach (var item in dupes)
            {
                ModInfo baseMod = new ModInfo(item.Value.ToString());
                ModInfo dupeMod = new ModInfo(item.Key.ToString());
                Expander dupeInfoExpander = new Expander()
                {
                    Header = baseMod.GetModName(),
                    Content = GetContent(baseMod, dupeMod)
                };
                dupePanel.Children.Add(dupeInfoExpander);
            }

            // Broken Symbols
            Expander brokenSymbolExpander = new Expander()
            {
                Header = "Broken Symbols",
                Content = new StackPanel()
            };
            similarInfoStackPanel.Children.Add(brokenSymbolExpander);

            var list = FileManager.GetBadFileNames();
            StackPanel brokenPanel = brokenSymbolExpander.Content as StackPanel;
            foreach (var item in list)
            {
                Expander expander = new Expander()
                {
                    Header = string.Join(", ", item.Key),
                    Content = item.Value
                };
                expander.MouseDoubleClick += expander_DoubleClick;
                brokenPanel.Children.Add(expander);
            }

            // Scrapped Mods
            Expander scrappedModsExpander = new Expander()
            {
                Header = "Scrapped Mods",
                Content = new StackPanel()
            };
            similarInfoStackPanel.Children.Add(scrappedModsExpander);

            var scraps = FileManager.GetDeletedScraps();
            StackPanel scrapPanel = scrappedModsExpander.Content as StackPanel;
            foreach (var item in scraps)
            {
                Expander scrapper = new Expander()
                {
                    Header = item.GetModName(),
                    Content = GetContent(item)
                };
                scrapper.MouseDoubleClick += expander_DoubleClick;
                scrapPanel.Children.Add(scrapper);
            }
        }
        private object GetContent(ModInfo baseMod, ModInfo dupeMod = null)
        {
            if (dupeMod is not null)
            {
                return dupeMod.GetModName() + "\nSortList: " + dupeMod.ExistsInSortData().ToString()
                                            + "\nMetaData: " + dupeMod.ExistsInMeta().ToString()
                                            + "\nModData: " + dupeMod.ExistsInModData().ToString()
                                            + "\nModFolder: " + dupeMod.ExistsInModFolder().ToString() + "\n"
                     + baseMod.GetModName() + "\nSortList: " + baseMod.ExistsInSortData().ToString()
                                            + "\nMetaData: " + baseMod.ExistsInMeta().ToString()
                                            + "\nModData: " + baseMod.ExistsInModData().ToString()
                                            + "\nModFolder: " + baseMod.ExistsInModFolder().ToString();
            }
            return baseMod.GetModName() + "\nSortList: " + baseMod.ExistsInSortData().ToString()
                                        + "\nMetaData: " + baseMod.ExistsInMeta().ToString()
                                        + "\nModData: " + baseMod.ExistsInModData().ToString()
                                        + "\nModFolder: " + baseMod.ExistsInModFolder().ToString();
        }
        private void expander_DoubleClick(object sender, RoutedEventArgs e)
        {
            // To successfully rename mod folders, the following needs to be done:
            // -- Rename the folder in FFXIVM
            // -- Rename the meta.json "name" field in the FFXIVM folder
            // -- Rename the temporary data mod_data .json folder
            // -- Rename the string relationship for the mod in sort_order.json
        }
    }
}
