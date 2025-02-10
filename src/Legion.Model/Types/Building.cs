using Legion.Model.Types.Definitions;

namespace Legion.Model.Types
{
    public class Building : TerrainObject
    {
        /// <summary>
        /// TYP=MIASTA(MIASTO,SNR,M_LUDZIE)
        /// </summary>
        public BuildingDefinition Type { get; set; }

        public int Width => Type.Width;

        public int Height => Type.Height;

        public int DoorsPos => Type.DoorsPos;

        // not used: MIASTA(MIASTO,SNR,M_PODATEK)
    }
}