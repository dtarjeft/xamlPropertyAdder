using System;
using System.Linq;
using System.Xml;

namespace XAMLPropertyAdder
{
    public class XmlParser
    {
        private XmlDocument Document { get; set; }
        private string Path { get; set; }
        private string[] WhiteListNodeNames { get; set; }

        public XmlParser(string path, string[] whiteListNodeNames)
        {
            Path = path;
            WhiteListNodeNames = whiteListNodeNames;
            Document = new XmlDocument();
            Document.Load(new XmlTextReader(path));
        }

        /// <summary>
        /// Edits and saves the .xaml/.xml document Document that was established when the class was initialized.
        /// Currently changes AutomationProperties "AutomationId" and "AutomationName".
        /// AutomationId is set to an empty string. TODO: Figure out what we want to do about this.
        /// TODO: The MSDN seems to indicate that AutomationId is supposed to be unique, while AutomationName performs more like a class in CSS/HMTL. 
        /// TODO: Not sure if I'm just understanding it wrong, I'm still pretty new to the UI Automation Process.
        /// AutomationName is set to the x:Name, skipping the first three characters. (Hungarian-> CamelCase) 
        /// TODO: Hope we were consistent w/ the Hungarian naming convention... I know there are labels that are x:Name'd, and I doubt we need the AutomationName for those. Hence the whitelist and filename changer.
        /// </summary>
        public void Parse(string toAppendToFilename)
        {
            if (!Document.HasChildNodes) return;
            var nodes = Document.ChildNodes;
            Parse(nodes);
            Document.Save(Path.Insert(Path.Count() - 5, toAppendToFilename));
        }

        private void Parse(XmlNodeList nodes)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].HasChildNodes)
                {
                    Parse(nodes[i].ChildNodes);
                }

                var xmlAttributeCollection = nodes[i].Attributes;

                if (xmlAttributeCollection == null)
                {
                    return;
                }
                //TODO: Fix this section so it's not hard-coded. This could be a pretty useful serialization tool with some customization options.
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
                    
                    ownerElement.SetAttribute("AutomationProperties.AutomationName",
                        "UI" + ownerElement.GetAttribute("x:Name").Substring(3));

                    ownerElement.SetAttribute("AutomationProperties.AutomationId", "");
                }
            }
        }
    }
}