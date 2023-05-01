using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace Synthesis.Managers
{
    public class ConfigManager
    {
        private static Configuration configuration;
        private static string StagingFolderPath;
        private const string PenumbraFolderPath = "XIVLauncher\\pluginConfigs\\Penumbra\\mod_data\\";
        private const string SortOrderFolderPath = "XIVLauncher\\pluginConfigs\\Penumbra\\";
        private static string NewModFolder = string.Empty;
        private static string PrimaryFolderSymbol = string.Empty;

        MainWindow window;
        public ConfigManager()
        {
            LoadAllConfig();
            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
        /* Main
         */
        public static void LoadAllConfig()
        {
            if (configuration is null)
            {
                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            if (configuration.AppSettings.Settings.Count > 0)
            {
                foreach (var setting in configuration.AppSettings.Settings.AllKeys)
                {
                    switch (setting)
                    {
                        case "stagingDir":
                            SetStagingDirectory(configuration.AppSettings.Settings[(string)setting].Value);
                            break;
                        case "newFolder":
                            SetNewModFolder(configuration.AppSettings.Settings[(string)setting].Value);
                            break;
                        case "primarySymbol":
                            SetPrimaryFolderSymbol(configuration.AppSettings.Settings[(string)setting].Value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public static void SetConfiguration()
        {
            configuration.Save(ConfigurationSaveMode.Modified);
        }
        public static bool GetPendingChanges()
        {
            var pendingSaves = IOManager.GetPendingSaves();
            if (pendingSaves > 0)
            {
                MessageBoxResult result = MessageBox.Show("You have " + pendingSaves + " pending changes.\nAre you sure you want to exit?",
                                              "Confirmation",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    configuration.Save(ConfigurationSaveMode.Modified);
                    Application.Current.Shutdown();
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /* Get
         */
        /// <summary>
        /// Returns the directory with individual mod configurations.
        /// </summary>
        /// <returns></returns>
        public static string GetPenumbraDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(
                                             Environment.SpecialFolder.ApplicationData).ToString(), PenumbraFolderPath);
        }
        /// <summary>
        /// Returns the directory where sort_order.json is located.
        /// </summary>
        /// <returns></returns>
        public static string GetSortOrderDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(
                                             Environment.SpecialFolder.ApplicationData).ToString(), SortOrderFolderPath);
        }
        /// <summary>
        /// Returns an array of all files and directories of the base meta directory.
        /// </summary>
        /// <returns></returns>
        public static string GetStagingDirectory()
        {
            if (StagingFolderPath is not null)
            {
                if (!StagingFolderPath.Equals(""))
                {
                    var dir = Directory.Exists(StagingFolderPath);

                    if (dir)
                    {
                        return StagingFolderPath;
                    }
                    else
                    {
                        MessageBox.Show("Invalid staging directory! Please re-configure.");
                    }
                }
            }
            else
            {
                LoadAllConfig();
                return StagingFolderPath;
            }
            return string.Empty;
        }
        /// <summary>
        /// Returns the main directory of mods.
        /// This includes both staging + main directories.
        /// </summary>
        /// <returns></returns>
        public static string GetMetaDirectory()
        {
            return GetStagingDirectory();
        }
        public static Configuration GetConfiguration()
        {
            return configuration;
        }
        public static string GetConfigValue(string key)
        {
            var config = GetConfiguration();
            var entry = config.AppSettings.Settings[key];

            if (entry is not null)
                return entry.Value;
            else
                return string.Empty;
        }
        /* Set
         */
        public static void WriteConfigValue(string key, string value)
        {
            var entry = configuration.AppSettings.Settings[key];
            if (entry == null)
                configuration.AppSettings.Settings.Add(key, value);
            else
                configuration.AppSettings.Settings[key].Value = value;
        }
        public static void SetStagingDirectory(string s)
        {
            WriteConfigValue("stagingDir", s);
            SetConfiguration();

            StagingFolderPath = s;
        }
        public static void SetPrimaryFolderSymbol(string s)
        {
            PrimaryFolderSymbol = s;
        }
        public static void SetNewModFolder(string s)
        {
            NewModFolder = s;
        }
        public static string GetPrimaryFolderSymbol()
        {
            return PrimaryFolderSymbol;
        }
        public static string GetNewModFolder()
        {
            return NewModFolder;
        }
    }
}
