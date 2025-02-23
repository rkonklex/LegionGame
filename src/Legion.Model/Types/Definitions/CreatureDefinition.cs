namespace Legion.Model.Types.Definitions
{
    public class CreatureDefinition : CharacterDefinition
    {
        /// <summary>RASY(I,3)</summary>
        public int P1 { get; set; }

        /// <summary>RASY(I,4)</summary>
        public int P2 { get; set; }

        /// <summary>RASY(I,5)</summary>
        public int Resistance { get; set; }

        /// <summary>RASY(I,6)</summary>
        public int Spell { get; set; }
    }
}