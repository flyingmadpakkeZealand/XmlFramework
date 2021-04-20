using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XmlFramework
{
    public static class XmlLoader
    {
        private static Dictionary<string, Tuple<string, XmlComponent>> _rootComponents;

        private static bool _clean = true;

        public static bool Clean
        {
            set => _clean = value;
            get => _clean;
        }

        public static XmlLayer Load(string fileName)
        {
            _rootComponents ??= new Dictionary<string, Tuple<string, XmlComponent>>();

            XmlLayer rootLayer;

            if (!_rootComponents.ContainsKey(fileName))
            {
                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                XmlNode root = document.DocumentElement;
                XmlComponent rootComponent = new XmlComponent(root);
                rootLayer = new XmlLayer(rootComponent, root.Name, fileName);
                _rootComponents.Add(fileName, new Tuple<string, XmlComponent>(root.Name, rootComponent));
            }
            else
            {
                rootLayer = new XmlLayer(_rootComponents[fileName].Item2, _rootComponents[fileName].Item1, fileName);
            }

            return rootLayer;
        }

        internal static void CleanRoots(string fileName)
        {
            _rootComponents?.Remove(fileName);

            if (_rootComponents?.Count == 0)
            {
                _rootComponents = null;
            }

        }
    }
}
