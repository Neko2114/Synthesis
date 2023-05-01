using Synthesis.Managers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Synthesis
{
    public partial class beginPage : Page
    {
        MainWindow window;
        public beginPage()
        {
            InitializeComponent();
            window = (MainWindow)Application.Current.MainWindow;
        }
        private void beginBtn(object sender, RoutedEventArgs e)
        {
        }
    }
}
