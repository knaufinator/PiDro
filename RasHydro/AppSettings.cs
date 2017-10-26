using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HydroTest
{
    public class AppSettings
    {

        XmlDocument settings;
 
        public static String settingsFileName = "settings.xml";
        private static String settingNodeName = "Settings";

        public AppSettings()
        {
            //if no settings file exists, create it.
            if (!File.Exists(settingsFileName))
            {
                CreateSettings();
            }

            LoadSettings();
        }

        private XmlNode FindNode(XmlElement list, string nodeName)
        {       
            foreach (XmlNode node in list)
            {
                if (node.Name.Equals(nodeName)) return node;
            }

            return null;
        }

        public String GetSetting(String nodeName,String attributeName,String defaultSetting)
        {
            String result = defaultSetting;
            XmlNode nodeFound = FindNode(settings[settingNodeName], nodeName);
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
            XmlNode node = settings.CreateNode(XmlNodeType.Element, name, null);
            settings[settingNodeName].AppendChild(node);
            return node;
        }

        private XmlAttribute CreateAttribute(XmlNode node,String name,String value)
        {
            XmlAttribute att = settings.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
            return att;
        }
        
        private void LoadSettings()
        {
            settings = new XmlDocument();
            settings.Load(settingsFileName);
        }

        private void CreateSettings()
        {
            XmlDocument settings = new XmlDocument();
            XmlElement root = settings.CreateElement(settingNodeName);
            settings.AppendChild(root);            
            settings.Save(settingsFileName);
        }

        internal void SaveSetting(string node, string setting, string value)
        {
            XmlNode settingNode = FindNode(settings[settingNodeName], node);
         
            //if node if null, create one,
            if (settingNode == null)
            {
                settingNode = CreateNode(node);
            }

            //find attribute on node, if null, create it
            XmlAttribute xmlAttribute = FindAttribute(settingNode.Attributes, setting);
            if (xmlAttribute == null)
            {
                CreateAttribute(settingNode, setting, value);
            }

            //set attribute value
            xmlAttribute.Value = value;

            //save settings file
            settings.Save(settingsFileName);
        }
    }
}


