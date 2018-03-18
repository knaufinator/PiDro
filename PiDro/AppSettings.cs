using Pidro.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// settings, saved as binary file.
/// </summary>
namespace Pidro
{
    public sealed class AppSettings
    {
        private static AppSettings instance = null;
        private static readonly object padlock = new object();

        private static String _settingsFileName = "settings.bin";
        private String _settingsPath = Directory.GetCurrentDirectory() + "/" + _settingsFileName;
        private static String _settingNodeName = "Settings";
        
        private static SettingClass currentSettings = new SettingClass();
        
        public static AppSettings Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AppSettings();
                    }
                    return instance;
                }
            }
        }

        public SettingClass CurrentSettings { get => currentSettings; set => currentSettings = value; }

        AppSettings()
        {
            try
            {
                //if no settings file exists, create it.
                if (!File.Exists(_settingsPath))
                {
                    Save();
                }

                LoadSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine("AppSettings init: " + e.Message);
            }
        }

        public void Save()
        {           
            Save( _settingsPath);
        }

        public void LoadSettings()
        {
            try
            {
                Stream stream = File.OpenRead(_settingsPath);

                Console.WriteLine(_settingsPath + " Opened");

                BinaryFormatter formatter = new BinaryFormatter();
                CurrentSettings = (SettingClass)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception err)
            {
                String test = "";
            }
        }

        public void SaveComponent(ComponentSetting item)
        {
            //check if exists,replace or add it.
            ComponentSetting setting = currentSettings.Components.FirstOrDefault(i => i.ID.CompareTo(item.ID) == 0);

            if (setting != null)
                currentSettings.Components.Remove(setting);

            currentSettings.Components.Add(item);
            
            Save();
        }

        public void DeleteSetting(Guid id)
        {
           var setting =  currentSettings.Components.FirstOrDefault(i => i.ID == id);

            if(setting != null)
            {
                currentSettings.Components.Remove(setting);
                Save();
            }
        }

        public void Save(string filePath)
        {
            try
            {
                Stream stream = File.Open(filePath, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, CurrentSettings);
                stream.Close();
            }
            catch (Exception e) //many more exception might happen, check documentation
            {
                String t = "";

            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }

        internal void ClearSettings()
        {
            CurrentSettings.Components.Clear();
            Save();
        }
    }

    [Serializable]
    public class SettingClass
    {
        private List<ComponentSetting> components = new List<ComponentSetting>();
        public List<ComponentSetting> Components { get => components; set => components = value; }

        internal object GetComponent(Guid iD)
        {
            return components.FirstOrDefault(i => i.ID == iD);
        }
    }
}


