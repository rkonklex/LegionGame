using System.Collections.Generic;

namespace Legion.Localization
{
    internal class LocalizedTexts
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Texts { get; set; }
    }
}