using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BattleSystem.Util
{
    public static class Utils
    {
        public static XmlDocument LoadXMLDocument(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
            XmlReader reader = XmlReader.Create(fileName, set);
            if (reader == null)
            {
                return null;
            }
            xml.Load(reader);
            reader.Close();
            return xml;
        }
    }
}
