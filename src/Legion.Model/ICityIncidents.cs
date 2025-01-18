using AwaitableCoroutine;
using Legion.Model.Types;

namespace Legion.Model
{
    public interface ICityIncidents
    {
        Coroutine Plague(City city, int type);
        Coroutine Riot(City city);
    }
}