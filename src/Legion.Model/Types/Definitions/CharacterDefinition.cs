using Legion.Utils;

namespace Legion.Model.Types.Definitions
{
    public abstract class CharacterDefinition
    {
        /// <summary>ID in the original game</summary>
        public int Oid { get; set; }
        public string Name { get; set; }
        public int Energy { get; set; }
        public int Strength { get; set; }
        public int Speed { get; set; }
        public string Img { get; set; }
    }
}