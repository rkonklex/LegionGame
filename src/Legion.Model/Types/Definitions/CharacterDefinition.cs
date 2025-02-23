using Legion.Utils;

namespace Legion.Model.Types.Definitions
{
    public abstract class CharacterDefinition
    {
        /// <summary>ID in the original game</summary>
        public int Oid { get; set; }

        /// <summary>RASY$(I)</summary>
        public string Name { get; set; }

        /// <summary>RASY(I,0)</summary>
        public int Energy { get; set; }

        /// <summary>RASY(I,1)</summary>
        public int Strength { get; set; }

        /// <summary>RASY(I,2)</summary>
        public int Speed { get; set; }

        /// <summary>RASY(I,7)</summary>
        public string Img { get; set; }
    }
}