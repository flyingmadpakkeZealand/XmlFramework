using System.Collections.Generic;

namespace XmlFramework
{
    public interface IExtraction
    {
        void Extract(Dictionary<string, List<XmlComponent>> componentsInLayer);
    }
}