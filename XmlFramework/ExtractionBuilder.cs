using System;
using System.Collections.Generic;
using System.Text;

namespace XmlFramework
{
    public class ExtractionBuilder
    {
        private Func<Dictionary<string, List<XmlComponent>>, bool> _onOne;
        private Func<Dictionary<string, List<XmlComponent>>, bool> _onMany;
        private Action _onNone;

        public ExtractionBuilder()
        {
        }

        public ExtractionBuilder OnNone(Action onNone)
        {
            _onNone = onNone;

            return this;
        }

        #region OnMany
        public ExtractionBuilder OnMany<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onMany, Action<TSetup> finish)
        {
            _onMany = components =>
            {
                TSetup set = setup();
                if (IterateOnMany(components, data => onMany(set, data)))
                {
                    finish(set);
                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionBuilder OnMany<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onMany)
        {
            _onMany = components =>
            {
                TSetup set = setup();
                return IterateOnMany(components, data => onMany(set, data));
            };

            return this;
        }

        public ExtractionBuilder OnMany(Action<List<XmlData>> onMany, Action finish)
        {
            _onMany = components =>
            {
                if (IterateOnMany(components, onMany))
                {
                    finish();
                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionBuilder OnMany(Action<List<XmlData>> onMany)
        {
            _onMany = components => IterateOnMany(components, onMany);

            return this;
        }
        #endregion

        #region OnOne
        public ExtractionBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne, Action<TSetup> finish)
        {
            _onOne = components =>
            {
                TSetup set = setup();
                if (IterateOnOne(components, data => onOne(set, data)))
                {
                    finish(set);
                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne)
        {
            _onOne = components =>
            {
                TSetup set = setup();
                return IterateOnOne(components, data => onOne(set, data));
            };

            return this;
        }

        public ExtractionBuilder OnOne(Action<List<XmlData>> onOne, Action finish)
        {
            _onOne = components =>
            {
                if (IterateOnOne(components, onOne))
                {
                    finish();
                    return true;
                }

                return false;
            };

            return this;
        }

        public ExtractionBuilder OnOne(Action<List<XmlData>> onOne)
        {
            _onOne = components => IterateOnOne(components, onOne);

            return this;
        }
        #endregion

        private static bool IterateOnMany(Dictionary<string, List<XmlComponent>> componentsInLayer, Action<List<XmlData>> onMany)
        {
            bool called = false;

            foreach (List<XmlComponent> components in componentsInLayer.Values)
            {
                foreach (XmlComponent component in components)
                {
                    if (component.RawData.Count != 0)
                    {
                        onMany(component.RawData);
                        called = true;
                    }
                }
            }

            return called;
        }

        private static bool IterateOnOne(Dictionary<string, List<XmlComponent>> componentsInLayer, Action<List<XmlData>> onOne)
        {
            foreach (List<XmlComponent> components in componentsInLayer.Values)
            {
                foreach (XmlComponent component in components)
                {
                    if (component.RawData.Count != 0)
                    {
                        onOne(component.RawData);
                        return true;
                    }
                }
            }

            return false;
        }

        public static implicit operator Extraction(ExtractionBuilder builder) => builder.Build();

        public Extraction Build()
        {
            return new Extraction(_onOne, _onMany, _onNone);
        }
    }
}
