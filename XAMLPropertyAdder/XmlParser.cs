using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace XAMLPropertyAdder
{
    public class XmlParser
    {
        private XmlDocument Document { get; set; }
        private string Path { get; set; }
        private string[] WhiteListNodeNames { get; set; }
        private Dictionary<string, string> PropertiesToChange { get; set; }

        public XmlParser(string path, string[] whiteListNodeNames)
        {
            Path = path;
            WhiteListNodeNames = whiteListNodeNames;
            Document = new XmlDocument();
            Document.Load(new XmlTextReader(path));
            
        }

        /// <summary>
        /// Edits and saves the .xaml/.xml document Document that was established when the class was initialized.
        /// Set rules for each property in propertiesToChangeList in XmlPropertyMethods.
        /// </summary>
        public void Parse(string toAppendToFilename, List<string> propertiesToChangeList)
        {
            if (!Document.HasChildNodes) return;
            var nodes = Document.ChildNodes;
            Parse(nodes, propertiesToChangeList);
            Document.Save(Path.Insert(Path.Count() - 5, toAppendToFilename));
        }

        private void Parse(XmlNodeList nodes, List<string> propertiesToChangeList )
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].HasChildNodes)
                {
                    Parse(nodes[i].ChildNodes, propertiesToChangeList);
                }

                var xmlAttributeCollection = nodes[i].Attributes;

                if (xmlAttributeCollection == null)
                {
                    return;
                }
                for (var j = 0; j < xmlAttributeCollection.Count; j++)
                {
                    var ownerElement = xmlAttributeCollection[j].OwnerElement;

                    if (ownerElement == null)
                    {
                        continue;
                    }

                    if (WhiteListNodeNames.All(name => String.Compare(name, ownerElement.Name, true) != 0))
                    {
                        continue;
                    }
                    PropertiesToChange = XmlPropertyMethods.Process(propertiesToChangeList, ownerElement, Path);
                    foreach (var property in PropertiesToChange)
                    {
                        ownerElement.SetAttribute(property.Key, property.Value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Currently changes AutomationProperties "AutomationId" and "AutomationName".
    /// AutomationId is set to an empty string. TODO: Figure out what we want to do about this.
    /// TODO: The MSDN indicates that AutomationId is supposed to be unique, while AutomationName performs more like a class in CSS/HMTL.
    /// TODO: Figure out if I'm mistaken or not. I found conflicting research in either case.
    /// AutomationName is set to the x:Name, skipping the first three characters. (Hungarian-> CamelCase) 
    /// TODO: Hope we were consistent w/ the Hungarian naming convention... There IS handling for inconsistent naming, but it could be better.
    /// </summary>
    public static class XmlPropertyMethods
    {
        public static Dictionary<string, string> Process(List<string> propertiesList, XmlElement elementToProcess, string xmlPath)
        {
            var res = propertiesList.ToDictionary(str => str);
            foreach (var s in propertiesList)
            {
                switch (s)
                {
                    case("AutomationProperties.AutomationName"):
                        try
                        {
                            if (elementToProcess.GetAttribute("x:Name").Substring(0,3).Any(char.IsUpper))
                            { throw new FormatException("Not so Hungarian...");}
                            res[s] = "UI" + elementToProcess.GetAttribute("x:Name").Substring(3);
                        }
                        catch
                        {
                            Console.WriteLine("Oddly formatted x:Name: " + elementToProcess.GetAttribute("x:Name") + " Element type: " + elementToProcess.Name +" File Loc: "+ xmlPath);
                            res[s] = "";
                        }
                        break;
                    //case("AutomationProperties.AutomationID"):
                    //    res[s] = "";
                    //    break;
                    default:
                        Console.WriteLine("No rules yet set for: " + s);
                        res[s] = "";
                        break;
                }
            }
            return res;
        }
    }
}