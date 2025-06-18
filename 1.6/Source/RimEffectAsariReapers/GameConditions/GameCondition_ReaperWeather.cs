using RimWorld;
using Verse;

namespace RimEffectAR
{
    public class GameCondition_ReaperWeather : GameCondition
    {
        public override WeatherDef ForcedWeather()
        {
            return RimEffectARDefOf.RER_ReaperLightningStorm;
        }
    }
}