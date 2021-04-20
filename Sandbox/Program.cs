using System;
using System.Collections.Generic;
using System.Xml;
using XmlFramework;

namespace Sandbox
{
    class Program
    {
        private static string _field;
        private const string FILE_BASE = @"C:\Users\Bruger\Desktop\SWC\ASWC\XmlFramework\Sandbox\";
        private const string FILE_TEST = FILE_BASE + "Test.xml";
        private const string FILE_SERVER_CONFIG = FILE_BASE + "ServerConfig.xml";

        static void Main(string[] args)
        {
            Testing();

            Console.ReadLine();
        }

        static void Testing()
        {
            XmlLayer rootLayer = XmlLoader.Load(FILE_TEST);

            XmlLayer newLayer = rootLayer.GoTo("Child", "InnerChild");
            XmlLayer layer = XmlLayer.CreateRootLayer(FILE_TEST);

            XmlLayer serverConfigRootLayer = XmlLayer.CreateRootLayer(FILE_SERVER_CONFIG);
            XmlLayer consoleFilterLayer = serverConfigRootLayer.GoTo("Logging", "Console", "Filters");
            XmlLayer jsonFilterLayer = serverConfigRootLayer.GoTo("Logging", "Json", "Filters");

            Extraction consoleExtraction = Extraction.OnMany((() => new List<string>()),
                    (list, data) => list.Add($"From: {data[0].Source}. {data[0].Data}"),
                    (list =>
                    {
                        foreach (string filter in list)
                        {
                            Console.WriteLine(filter);
                        }
                    }))
                .OnOne(data => Console.WriteLine($"One Filter was found, {data[0].Source}. {data[0].Data}"))
                .OnNone((() => Console.WriteLine("No filters detected.")));

            consoleFilterLayer.ExtractIntoFromElements(consoleExtraction);

            ExtractionRaw raw = ExtractionRaw.OnOne(data => Console.WriteLine($"From {data[0].Source}. {data[0].Data}"))
                .OnNone(() => Console.WriteLine("No raw data found."));

            serverConfigRootLayer.ExtractIntoFromRaw(raw, "General.Port");

            layer.Dispose();

            layer = XmlLoader.Load(FILE_TEST);
            layer = layer.CreateRootLayer();
        }
    }
}
