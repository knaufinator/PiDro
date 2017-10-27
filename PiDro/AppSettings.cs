using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

/// <summary>
/// Custom settings xml writer, create and edit settings dynamically.
/// </summary>
namespace HydroTest
{
    public sealed class AppSettings
    {
        private static AppSettings instance = null;
        private static readonly object padlock = new object();

        private XmlDocument _settings;
        private static String _settingsFileName = "settings.xml";
        private String _settingsPath = Directory.GetCurrentDirectory() + "/" + _settingsFileName;
        private static String _settingNodeName = "Settings";

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
              
        AppSettings()
        {
            try
            {
                //if no settings file exists, create it.
                if (!File.Exists(_settingsPath))
                {
                    CreateSettings();
                }

                LoadSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine("AppSettings init: "+e.Message);
            }
        }

        private XmlNode FindNode(XmlElement list, string nodeName)
        {
            try
            {
                foreach (XmlNode node in list)
                {
                    Console.WriteLine("FindNode "+ nodeName + " next:" + node.ToString());
                    if (node.Name.Equals(nodeName)) return node;
                }

            }
            catch (Exception er)
            {
                Console.WriteLine("Find Node " +er.Message);
            }
            
            return null;
        }

        public String GetSetting(String nodeName,String attributeName,String defaultSetting)
        {

            Console.WriteLine("get Setting " + nodeName + "Settings null?: "+ (_settings == null).ToString());

            String result = defaultSetting;
            XmlNode nodeFound = FindNode(_settings[_settingNodeName], nodeName);
            XmlAttribute xmlAttribute;

            if (nodeFound != null)
            {
                xmlAttribute = FindAttribute(nodeFound.Attributes, attributeName);

                if (xmlAttribute != null)
                    result = xmlAttribute.Value;
            }

            return result;
        }

        private XmlAttribute FindAttribute(XmlAttributeCollection attributes, string attributeName)
        {
            XmlAttribute xmlAttribute = null;

            if (attributes != null)
            {
                if (attributes[attributeName] != null)
                {
                    xmlAttribute = attributes[attributeName];
                }
            }

            return xmlAttribute;
        }

        private XmlNode CreateNode(String name)
        {
            XmlNode node = _settings.CreateNode(XmlNodeType.Element, name, null);
            _settings[_settingNodeName].AppendChild(node);
            return node;
        }

        private XmlAttribute CreateAttribute(XmlNode node,String name,String value)
        {
            XmlAttribute att = _settings.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
            return att;
        }
        
        private void LoadSettings()
        {
            _settings = new XmlDocument();
            _settings.Load(_settingsPath);
            Console.WriteLine(_settingsPath + " Loaded");
        }

        private void CreateSettings()
        {
            Console.WriteLine("Creating settings file");
            XmlDocument settings = new XmlDocument();
            XmlElement root = settings.CreateElement(_settingNodeName);
            settings.AppendChild(root);      
            settings.Save(_settingsFileName);
            Console.WriteLine("Created settings file: "+ _settingsFileName);
        }

        internal void SaveSetting(string node, string setting, string value)
        {
            XmlNode settingNode = FindNode(_settings[_settingNodeName], node);
         
            //if node if null, create one,
            if (settingNode == null)
            {
                settingNode = CreateNode(node);
            }

            //find attribute on node, if null, create it
            XmlAttribute xmlAttribute = FindAttribute(settingNode.Attributes, setting);
            if (xmlAttribute == null)
            {
                xmlAttribute = CreateAttribute(settingNode, setting, value);
            }

            //set attribute value
            xmlAttribute.Value = value;

            //save settings file
            _settings.Save(_settingsPath);
        }
    }
}


