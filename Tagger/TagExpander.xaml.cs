using Penumbra_.Pages;
using Synthesis.Managers;
using Synthesis.ModInformation;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;

namespace Synthesis
{
    public partial class TagExpander : Page
    {
        MainWindow window;
        public TagExpander()
        {
            InitializeComponent();

            foreach (var element in DataManager.GetPageDataElements())
            {
                Expander Header = new Expander()
                {
                    Header = element,
                    Content = new StackPanel()
                };

                var headerPanel = Header.Content as StackPanel;
                var buttonGrid = new Grid() { Name = "buttonGrid" };

                headerPanel.Children.Add(buttonGrid);

                if (element.Equals("presets"))
                {
                    TagButtons.SetupPresets(buttonGrid, this, DataManager.GetPresetData(element), 0, "preset");
                }
                else
                {
                    TagButtons.SetupModButtons(buttonGrid, this, DataManager.GetPageData(element), 0);
                }

                modStackPanel.Children.Add(Header);

            }
        }
    }
}
