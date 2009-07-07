using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace EasyMYP
{
    public static class EasyMypConfig
    {
        #region Attributes

        private static string extractionPath = "ExtractionPath";

        #endregion

        #region Properties

        public static string ExtractionPath
        {
            get { return ConfigurationManager.AppSettings[extractionPath]; }
            set
            {
                if (value == null || value == "" || !Directory.Exists(value))
                {
                    RemoveConfigurationKey(extractionPath);
                }
                else
                {
                    UpdateConfiguration(extractionPath, value);
                }
            }
        }

        #endregion

        #region Functions

        static void UpdateConfiguration(string key, string value)
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Check to see if the key already exists, if so we remove it
            if (ConfigurationManager.AppSettings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
            }
            // Add the new key value
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        static void RemoveConfigurationKey(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Check to see if the key already exists, if so we remove it
            for (int i = 0; i < ConfigurationManager.AppSettings.Keys.Count; i++)
            {
                if (ConfigurationManager.AppSettings.Keys.Get(i) == key)
                {
                    config.AppSettings.Settings.Remove(key);
                    break;
                }
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        #endregion
    }
}
