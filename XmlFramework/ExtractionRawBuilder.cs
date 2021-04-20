using System;
using System.Collections.Generic;
using System.Text;

namespace XmlFramework
{
    public class ExtractionRawBuilder
    {
        private Func<List<XmlData>, bool> _onOne;
        private Action _onNone;

        public ExtractionRawBuilder()
        {
            
        }

        public ExtractionRawBuilder OnNone(Action onNone)
        {
            _onNone = onNone;

            return this;
        }

        public ExtractionRawBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne, Action<TSetup> finish)
        {
            _onOne = rawData =>
            {
                if (rawData.Count != 0)
                {
                    TSetup set = setup();
                    onOne(set, rawData);
                    finish(set);

                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionRawBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne)
        {
            _onOne = rawData =>
            {
                if (rawData.Count != 0)
                {
                    TSetup set = setup();
                    onOne(set, rawData);

                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionRawBuilder OnOne(Action<List<XmlData>> onOne, Action finish)
        {
            _onOne = rawData =>
            {
                if (rawData.Count != 0)
                {
                    onOne(rawData);
                    finish();

                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionRawBuilder OnOne(Action<List<XmlData>> onOne)
        {
            _onOne = rawData =>
            {
                if (rawData.Count != 0)
                {
                    onOne(rawData);

                    return true;
                }

                return false;
            };

            return this;
        }

        public static implicit operator ExtractionRaw(ExtractionRawBuilder builder) => builder.Build();

        public ExtractionRaw Build()
        {
            return new ExtractionRaw(_onOne, _onNone);
        }
    }
}
