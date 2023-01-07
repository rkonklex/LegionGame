using Legion.Model.Types;

namespace Legion.Model
{
    public interface ICityIncidents
    {
        void Plague(City city, int type);
        void Riot(City city);
    }
}