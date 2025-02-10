namespace Legion.Model.Types
{
    public class Decoration : TerrainObject
    {
        public Decoration(int x, int y, int bob, bool hrev)
        {
            X = x;
            Y = y;
            Bob = bob;
            Hrev = hrev;
        }

        public int Bob { get; set; }
        public bool Hrev { get; set; }
    }
}