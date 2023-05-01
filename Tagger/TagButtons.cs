using Synthesis;
using Synthesis.Managers;
using Synthesis.Pages;
using Synthesis.Penumbra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Penumbra_.Pages
{
    public class TagButtons
    {
        public static void SetupPresets(Grid buttonGrid, TagExpander page, string[][] data, int buttonStartIndex, string buttonType = "normal")
        {
            Button button = null;
            int COLUMN_MAX = 6;
            int ROW_MAX = 7;

            var buttonCount = 0;
            var rowIndex = 0;
            var columnIndex = 0;
            var arrayIndex = buttonStartIndex; // buttonStartIndex is the offset per page.

            while (buttonStartIndex < data.Count())
            {
                buttonCount++;
                button = new Button()
                {
                    Content = data[arrayIndex][0],
                    Width = 75,
                    Height = 27,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                if (button.Tag == null) button.Tag = false;

                if (buttonType.Equals("preset"))
                {
                    button.Click += (s, e) => {
                        Button btn = s as Button;

                        var data = DataManager.GetPresetButtonData(btn.Content.ToString());
                        DataManager.GetTagList().AddRange(data.GetTags());

                        IOManager.SaveSortPathLocally(data.GetDir(), DataManager.GetSelectedItem().objectName);
                    };
                }
                else
                {
                    button.Click += (s, e) => {
                        Button btn = s as Button;

                        if ((bool)btn.Tag == false)
                        {
                            btn.Background = Brushes.DarkGray;
                            btn.Tag = true;
                            Keyboard.ClearFocus();
                        }
                        else
                        {
                            if ((bool)btn.Tag == true)
                            {
                                btn.Tag = false;
                                btn.ClearValue(Control.BackgroundProperty);
                                Keyboard.ClearFocus();
                            }
                        }
                    };
                }


                CreateColumns(buttonGrid, 1);
                if (buttonGrid.RowDefinitions.Count == 0)
                {
                    CreateRows(buttonGrid, 1);
                }

                Grid.SetColumn(button, columnIndex);
                Grid.SetRow(button, buttonGrid.RowDefinitions.Count - 1);

                buttonGrid.Children.Add(button);

                columnIndex++;
                if (columnIndex >= COLUMN_MAX)
                {
                    columnIndex = 0;
                    rowIndex++;
                    CreateRows(buttonGrid, 1);
                }

                arrayIndex++;
                if (buttonCount + buttonStartIndex >= data.Count())
                {
                    break; // Break if the total buttons equal the max amount from data.
                }
            }
        }
        public static void SetupModButtons(Grid buttonGrid, TagExpander page, string[] data, int buttonStartIndex)
        {
            Button button = null;
            int COLUMN_MAX = 6;
            int ROW_MAX = 7;

            var buttonCount = 0;
            var rowIndex = 0;
            var columnIndex = 0;

            while (buttonStartIndex < data.Count())
            {
                buttonCount++;
                button = new Button()
                {
                    Content = data[buttonGrid.Children.Count + buttonStartIndex],
                    Width = 75,
                    Height = 27,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                if (button.Tag == null) button.Tag = false;

                button.Click += (s, e) => {
                    Button btn = s as Button;

                    if ((bool)btn.Tag == false)
                    {
                        btn.Background = Brushes.DarkGray;
                        btn.Tag = true;
                        Keyboard.ClearFocus();
                    }
                    else
                    {
                        if ((bool)btn.Tag == true)
                        {
                            btn.Tag = false;
                            btn.ClearValue(Control.BackgroundProperty);
                            Keyboard.ClearFocus();
                        }
                    }
                };
                CreateColumns(buttonGrid, 1);
                if (buttonGrid.RowDefinitions.Count == 0)
                {
                    CreateRows(buttonGrid, 1);
                }

                Grid.SetColumn(button, columnIndex);
                Grid.SetRow(button, buttonGrid.RowDefinitions.Count - 1);

                buttonGrid.Children.Add(button);

                columnIndex++;
                if (columnIndex >= COLUMN_MAX)
                {
                    columnIndex = 0;
                    rowIndex++;
                    CreateRows(buttonGrid, 1);
                }
                
                if (buttonCount + buttonStartIndex >= data.Count())
                {
                    break; // Break if the total buttons equal the max amount from data.
                }
            }
        }
        private static void CreateColumns(Grid buttonGrid, int count)
        {
            for (var i = 0; i < count; i++)
            {
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                });
            }
        }
        private static void CreateRows(Grid buttonGrid, int count)
        {
            for (var i = 0; i < count; i++)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                });
            }
        }
        public static List<string> SetupCollectionsButton(Grid buttonGrid, ObservableCollection<PenumbraObjectData> data)
        {
            var totalSize = data.Count();

            var buttonIndex = 0;
            var columnIndex = 0;
            var authorList = new List<string>();

            foreach (var mod in data)
            {
                Button button = null;

                authorList.Add(mod.author);

                button = new Button()
                {
                    Content = "\n\n" + mod.author + "\n\n\"" + mod.objectName + "\"",
                    Width = 180,
                    Height = 150,
                    Style = buttonGrid.FindResource("ButtonStyle") as Style,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(button.Width + 10, GridUnitType.Pixel)
                });
                if (buttonGrid.RowDefinitions.Count == 0)
                {
                    buttonGrid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(button.Height + 10, GridUnitType.Pixel)
                    });
                }

                Grid.SetColumn(button, buttonIndex);
                Grid.SetRow(button, columnIndex);

                buttonGrid.Children.Add(button);

                buttonIndex++;

                if (buttonIndex > 4)
                {
                    columnIndex++;
                    buttonIndex = 0;

                    buttonGrid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(button.Height + 10, GridUnitType.Pixel)
                    });
                }
            }
            return authorList;
        }
    }
}
