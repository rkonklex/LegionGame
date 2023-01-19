using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Legion.Localization
{
    public class Texts : ITexts
    {
        private static readonly string FilePath = Path.Combine("data", "texts", "texts.{0}.json");

        private Dictionary<string, string> _localizedTexts;
        private readonly ILanguageProvider _languageProvider;

        public Texts(ILanguageProvider languageProvider)
        {
            _languageProvider = languageProvider;
            Load(languageProvider.Language);
            languageProvider.LanguageChanged += lang => Load(lang);
        }

        private void Load(string language)
        {
            var textsJson = File.ReadAllText(string.Format(FilePath, language));
            var loadedTexts = JsonConvert.DeserializeObject<LocalizedTexts>(textsJson);
            if (loadedTexts == null)
            {
                throw new Exception("Unable to load texts for language " + language);
            }
            _localizedTexts = new Dictionary<string, string>(loadedTexts.Texts, StringComparer.InvariantCultureIgnoreCase);
        }

        public string Get(string key, params object[] args)
        {
            string text;
            if (!_localizedTexts.TryGetValue(key, out text))
            {
                return string.Format("x:{0}", key);
            }

            if (args != null && args.Length > 0)
            {
                text = string.Format(text, args);
            }

            return text;
        }
    }
}