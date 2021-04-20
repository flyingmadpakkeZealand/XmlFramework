using System;
using System.Collections.Generic;

namespace XmlFramework
{
    public class ExtractionRaw : IExtractionRaw
    {
        private readonly Func<List<XmlData>, bool> _onOne;
        private readonly Action _onNone;

        internal ExtractionRaw(Func<List<XmlData>, bool> onOne, Action onNone)
        {
            _onOne = onOne;
            _onNone = onNone;
        }

        public static ExtractionRawBuilder OnNone(Action onNone)
        {
            return new ExtractionRawBuilder().OnNone(onNone);
        }

        public static ExtractionRawBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne, Action<TSetup> finish)
        {
            return new ExtractionRawBuilder().OnOne(setup, onOne, finish);
        }

        public static ExtractionRawBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne)
        {
            return new ExtractionRawBuilder().OnOne(setup, onOne);
        }

        public static ExtractionRawBuilder OnOne(Action<List<XmlData>> onOne, Action finish)
        {
            return new ExtractionRawBuilder().OnOne(onOne, finish);
        }

        public static ExtractionRawBuilder OnOne(Action<List<XmlData>> onOne)
        {
            return new ExtractionRawBuilder().OnOne(onOne);
        }

        public void ExtractIntoFromRaw(List<XmlData> rawData)
        {
            if (!(_onOne is null) && _onOne(rawData))
            {
                return;
            }

            _onNone?.Invoke();
        }
    }
}
