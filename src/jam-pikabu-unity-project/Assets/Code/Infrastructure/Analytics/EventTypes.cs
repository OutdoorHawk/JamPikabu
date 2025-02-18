namespace Code.Infrastructure.Analytics
{
    public static class AnalyticsEventTypes
    {
        public const string LevelStart = "LEVEL_START";
        public const string LevelEnd = "LEVEL_END";
        public const string StarsEarned = "STARS_EARNED";
        public const string MainMenuEnter = "MAIN_MENU_ENTER";
        public const string UpgradeLootType = "INGREDIENT_UPGRADE";
        public const string UpgradeLootTypeFree = "INGREDIENT_UPGRADE_FREE";
        public const string LootUpgraded = "INGREDIENT_UPGRADED";
        public const string AdStarted = "ADS_START";
        public const string AdRewardedSuccess = "ADS_REWARD_SUCCESS";
        public const string Purchase = "PURCHASE_CONSUMABLE";
        public const string DoubleProfitReward = "DOUBLE_PROFIT";
    }

    public enum AdsEventTypes
    {
        Unknown = 0,
        Interstitial = 1,
        Banner = 2,
        Preload = 3,
        Rewarded = 4,
    }
}