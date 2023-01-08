namespace Legion.Model.Types
{
    public abstract class MapObject
    {
        public int Id { get; set; }

        public abstract MapObjectType Type { get; }

        public Player Owner { get; set; }

        public string Name { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>True if the Owner is a Player that represents the User.</summary>
        public bool IsUserControlled => Owner?.IsUserControlled ?? false;

        /// <summary>True if the Owner is a Player that represents the Chaos.</summary>
        public bool IsChaosControlled => Owner?.IsChaosControlled ?? false;

        /// <summary>True if the Owner is a Player that does not represent the User nor the Chaos.</summary>
        public bool IsRivalControlled => Owner?.IsRivalControlled ?? false;
    }
}