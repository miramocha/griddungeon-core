namespace GridDungeon.Core.Hub
{
    /// <summary>MVP1 hub service defaults (#13). Content SOs may override later.</summary>
    public static class Mvp1HubConstants
    {
        public const string DefaultNavigatorId = "guild_handler";

        public const string CurrencyDisplayName = "Credits";

        public static string FormatAmount(int amount) => $"{amount} {CurrencyDisplayName}";

        public static string FormatWalletLabel(int balance) => $"{CurrencyDisplayName}: {balance}";

        public static string NotEnoughCreditsTo(string action) =>
            $"Not enough {CurrencyDisplayName} to {action}.";

        public const int HealPartyBaseCost = 40;
        public const int HealPartyPerMissingHp = 2;
        public const int ReviveBaseCost = 120;

        public const int StartingHubCredits = 500;
        public const string ShopStubConsumableId = "patch_kit";
        public const int ShopStubConsumablePrice = 25;
        public const int ShopSellStubPrice = 12;

        public const int ShopIdentifyCost = 25;

        public static readonly string[] DayOneClassIds =
        {
            "vanguard",
            "breaker",
            "medic",
            "summoner",
            "marksman",
            "tactician",
        };
    }
}
