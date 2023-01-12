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

        FireInTheCity,
        EpidemyInTheCity,
        RatsInTheCity,
        RiotInTheCity,
        RiotInTheCityLost,
        RiotInTheCitySuccess,
        RiotInTheCityWithDefence,
    }
}