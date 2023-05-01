using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Synthesis
{
    public partial class InputWindow : Window
    {
        public static string returnVal = "";
        public InputWindow(string s)
        {
            InitializeComponent();
            if (s.Equals("NewFolder"))
            {
                Title = "Select new mod folder path";
            }
            else if (s.Equals("PrimarySymbol"))
            {
                Title = "Select folder exclusion symbol";
            }
        }
        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            returnVal = optionText.Text;
            Close();
        }
    }
}