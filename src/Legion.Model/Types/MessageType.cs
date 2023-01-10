namespace Legion.Model.Types
{
    public enum MessageType
    {
        UserAttackCity,
        UserCapturedCity,
        ArmyDestroyedCapturingCity,
        EnemyAttacksUserCity,
        EnemyCapturedUserCity,
        EnemyConqueredUserCity,
        EnemyConqueredCity,
        ChaosWarriorsBurnedCity,

        UserAttacksArmy,
        EnemyAttacksUserArmy,
        ArmyDestroyed,
        ArmyTrackedDownBeast,

        ArmyEncounteredRabidWolves,
        ArmyEncounteredBandits,
        ArmyStuckInSwamp,
        ArmyEncounteredForestTrolls,
        ArmyEncounteredGargoyl,
        ArmyEncounteredLoneKnight,
        ArmyEncounteredCaveEntrance,

        FireInTheCity,
        EpidemyInTheCity,
        RatsInTheCity,
        RiotInTheCity,
        RiotInTheCityLost,
        RiotInTheCitySuccess,
        RiotInTheCityWithDefence,
    }
}