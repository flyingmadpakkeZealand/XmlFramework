using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlFramework
{
    public class XmlComponent
    {
        public List<XmlData> RawData { get; }
        public Dictionary<string, List<XmlComponent>> Children { get; }

        internal XmlComponent(XmlNode next)
        {
            RawData = new List<XmlData>();
            Children = new Dictionary<string, List<XmlComponent>>();

            XmlNodeList children = next.ChildNodes;

            foreach (XmlNode child in children)
            {
                switch (child)
                {
                    case XmlElement element when !(element.HasAttribute("ignore") && element.GetAttribute("ignore").Trim().ToLower() == "true"):
                        AddElementToComponent(this, element); break;
                    case XmlText text:
                        RawData.Add(new XmlData(next.Name, text.Value.Trim())); break;
                }
            }
        }

        private void AddElementToComponent(XmlComponent component, XmlElement element)
        {
            XmlComponent newComponent = new XmlComponent(element);
            if (!component.Children.TryAdd(element.Name, new List<XmlComponent>() { newComponent }))
            {
                component.Children[element.Name].Add(newComponent);
            }
        }
    }
}
