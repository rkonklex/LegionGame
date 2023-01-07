namespace Legion.Model
{
    public class LegionConfig : ILegionConfig
    {
        public int PlayersCount { get; private set; } = 5;

        // For M=0 To 49: LUDZIE=MIASTA(M,0,M_LUDZIE)
        public int MaxCitiesCount { get; private set; } = 50;

        // For I=2 To 20: TYP=MIASTA(NR,I,M_LUDZIE)
        public int MaxCityBuildingsCount { get; private set; } = 19;

        public int WorldWidth { get; private set; } = 640;
        public int WorldHeight { get; private set; } = 512;

        public bool GoDmOdE { get; private set; } = false;
    }
}