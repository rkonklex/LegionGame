namespace Legion.Model.Types
{
    public enum MessageType
    {
        UserAttackCity,
        UserCapturedCity,
        UserArmyFailedToCaptureCity,
        EnemyAttacksUserCity,
        EnemyCapturedUserCity,
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