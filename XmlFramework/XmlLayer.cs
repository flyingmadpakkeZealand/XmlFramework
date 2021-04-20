using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace XmlFramework
{
    public class XmlLayer : IDisposable
    {
        private string _name;
        private List<XmlData> _rawData;
        private Dictionary<string, List<XmlComponent>> _components;
        private readonly string _fileName;

        public string Name => _name;

        public List<XmlData> RawData => _rawData;

        public static XmlLayer CreateRootLayer(string fileName)
        {
            return XmlLoader.Load(fileName);
        }

        public static XmlLayer CreateEmptyLayer()
        {
            return new XmlLayer("Empty", null);
        }

        public XmlLayer CreateRootLayer()
        {
            return XmlLoader.Load(_fileName);
        }

        internal XmlLayer(XmlComponent rootComponent, string name, string fileName)
        {
            _rawData = rootComponent.RawData;
            _components = rootComponent.Children;
            _name = name;
            _fileName = fileName;
        }

        private XmlLayer(string name, string fileName)
        {
            _rawData = new List<XmlData>();
            _components = new Dictionary<string, List<XmlComponent>>();
            _name = name;
            _fileName = fileName;
        }

        public XmlLayer GoTo([NotNull] string path)
        {
            return GoTo(path.Split('.', StringSplitOptions.RemoveEmptyEntries));
        }

        public XmlLayer GoTo(params string[] pathFinding)
        {
            if (pathFinding.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pathFinding), "Empty paths are invalid");
            }

            XmlLayer newLayer = new XmlLayer(pathFinding[^1], _fileName);

            string startKey = pathFinding[0];
            if (_components.ContainsKey(startKey))
            {
                foreach (XmlComponent component in _components[startKey])
                {
                    PathFind(1, component);
                }
            }

            return newLayer;




            void PathFind(int advance, XmlComponent fromComponent)
            {
                if (advance == pathFinding.Length)
                {
                    newLayer._rawData.AddRange(fromComponent.RawData);

                    foreach (KeyValuePair<string, List<XmlComponent>> child in fromComponent.Children)
                    {
                        if (!newLayer._components.TryAdd(child.Key, child.Value))
                        {
                            newLayer._components[child.Key].AddRange(child.Value);
                        }
                    }
                }
                else if (fromComponent.Children.TryGetValue(pathFinding[advance], out List<XmlComponent> matchChildren))
                {
                    foreach (XmlComponent component in matchChildren)
                    {
                        PathFind(++advance, component);
                    }
                }
            }
        }

        public void ExtractIntoFromElements(IExtraction extraction, string path)
        {
            ExtractIntoFromElements(extraction, path.Split('.', StringSplitOptions.RemoveEmptyEntries));
        }

        public void ExtractIntoFromElements(IExtraction extraction, params string[] pathFinding)
        {
            XmlLayer reference = this;
            if (pathFinding.Length != 0)
            {
                reference = GoTo(pathFinding);
            }

            extraction.Extract(reference._components);
        }

        public void ExtractIntoFromRaw(IExtractionRaw extraction, string path)
        {
            ExtractIntoFromRaw(extraction, path.Split('.', StringSplitOptions.RemoveEmptyEntries));
        }

        public void ExtractIntoFromRaw(IExtractionRaw extraction, params string[] pathFinding)
        {
            XmlLayer reference = this;
            if (pathFinding.Length != 0)
            {
                reference = GoTo(pathFinding);
            }

            extraction.ExtractIntoFromRaw(reference._rawData);
        }

        //~XmlLayer()
        //{
        //    if (XmlLoader.Clean)
        //    {
        //        XmlLoader.CleanRoots(_fileName);
        //    }
        //}

        public void Dispose()
        {
            if (!(_fileName is null))
            {
                XmlLoader.CleanRoots(_fileName);
            }
        }
    }
}
