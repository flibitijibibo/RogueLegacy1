using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class XMLCompiler
    {
#if !XBOX
        public static void CompileTriggers(List<TriggerAction> triggerActionList, string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filePath + System.IO.Path.DirectorySeparatorChar + "testoutput.xml", settings);


            string xmlStringtest = "<xml>";
            writer.WriteStartElement("xml");

            foreach (TriggerAction trigger in triggerActionList)
            {
                Type type = trigger.GetType();
                ConstructorInfo[] ctors = type.GetConstructors();
                xmlStringtest += "<TriggerAction>\n";
                writer.WriteStartElement("TriggerAction");

                foreach (ConstructorInfo ctor in ctors)
                {
                    ParameterInfo[] parameterList = ctor.GetParameters();

                    writer.WriteStartElement("Constructor");
                    writer.WriteAttributeString("name", type.ToString().Substring(type.ToString().LastIndexOf(".") + 1));
                    xmlStringtest += "<constructor name=" + type.ToString().Substring(type.ToString().LastIndexOf(".") + 1) + ">\n";

                    foreach (ParameterInfo parameter in parameterList)
                    {
                        writer.WriteStartElement("Parameter");
                        writer.WriteAttributeString("type", parameter.ParameterType.ToString().Substring(parameter.ParameterType.ToString().LastIndexOf(".") + 1));
                        writer.WriteAttributeString("name", parameter.Name);
                        xmlStringtest += "<parameter type=" + parameter.ParameterType.ToString().Substring(parameter.ParameterType.ToString().LastIndexOf(".") + 1) + " />\n";
                        writer.WriteEndElement();
                    }

                    xmlStringtest += "</constructor>\n";
                    writer.WriteEndElement();
                }
                
                writer.WriteEndElement();
                xmlStringtest += "</TriggerAction>\n";
            }
            writer.WriteEndElement();
           // Console.WriteLine(xmlStringtest);
            writer.Flush();
            writer.Close();

        }

        public static List<string> ParseTriggerFile(string filePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            XmlReader reader = XmlReader.Create(filePath, settings);

            List<string> stringListToReturn = new List<string>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Constructor")
                    {
                        reader.MoveToAttribute("name");
                        stringListToReturn.Add("Constructor:" + reader.Value);
                    }
                    else if (reader.Name == "Parameter")
                    {
                        reader.MoveToAttribute("type");
                        stringListToReturn.Add("Parameter:" + reader.Value);
                    }
                }
            }

            return stringListToReturn;
        }
#endif
    }
}
