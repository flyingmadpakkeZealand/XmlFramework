using System;
using System.Collections.Generic;
using System.Text;

namespace XmlFramework
{
    public class XmlData
    {
        public string Source { get; }
        public string Data { get; }

        internal XmlData(string source, string data)
        {
            Source = source;
            Data = data;
        }
    }
}
