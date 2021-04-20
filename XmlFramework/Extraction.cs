using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlFramework
{
    public class Extraction : IExtraction
    {
        private Func<Dictionary<string, List<XmlComponent>>, bool> _onOne;
        private Func<Dictionary<string, List<XmlComponent>>, bool> _onMany;
        private Action _onNone;

        internal Extraction(Func<Dictionary<string, List<XmlComponent>>, bool> onOne, Func<Dictionary<string, List<XmlComponent>>, bool> onMany, Action onNone)
        {
            _onOne = onOne;
            _onMany = onMany;
            _onNone = onNone;
        }

        public static ExtractionBuilder OnNone(Action onNone)
        {
            return new ExtractionBuilder().OnNone(onNone);
        }

        #region OnMany
        public static ExtractionBuilder OnMany<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onMany, Action<TSetup> finish)
        {
            return new ExtractionBuilder().OnMany(setup, onMany, finish);
        }

        public static ExtractionBuilder OnMany<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onMany)
        {
            return new ExtractionBuilder().OnMany(setup, onMany);
        }

        public static ExtractionBuilder OnMany(Action<List<XmlData>> onMany, Action finish)
        {
            return new ExtractionBuilder().OnMany(onMany, finish);
        }

        public static ExtractionBuilder OnMany(Action<List<XmlData>> onMany)
        {
            return new ExtractionBuilder().OnMany(onMany);
        }
        #endregion

        #region OnOne
        public static ExtractionBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne, Action<TSetup> finish)
        {
            return new ExtractionBuilder().OnOne(setup, onOne, finish);
        }

        public static ExtractionBuilder OnOne<TSetup>(Func<TSetup> setup, Action<TSetup, List<XmlData>> onOne)
        {
            return new ExtractionBuilder().OnOne(setup, onOne);
        }

        public static ExtractionBuilder OnOne(Action<List<XmlData>> onOne, Action finish)
        {
            return new ExtractionBuilder().OnOne(onOne, finish);
        }

        public static ExtractionBuilder OnOne(Action<List<XmlData>> onOne)
        {
            return new ExtractionBuilder().OnOne(onOne);
        }
        #endregion

        public void Extract(Dictionary<string, List<XmlComponent>> componentsInLayer)
        {
            if (!(_onOne is null) && OnOneCalled(componentsInLayer))
            {
                return;
            }

            if (!(_onMany is null) && _onMany(componentsInLayer))
            {
                return;
            }

            _onNone?.Invoke();
        }

        private bool OnOneCalled(Dictionary<string, List<XmlComponent>> componentsInLayer)
        {
            Dictionary<string, List<XmlComponent>>.ValueCollection components = componentsInLayer.Values;

            if (components.Count < 1 || !(_onMany is null || components.First().Count == 1 && components.Count == 1))
            {
                return false;
            }

            return _onOne(componentsInLayer);
        }
    }
}
