namespace Content.Server.Imperial.CTF.Events
{
    public sealed class OnPlayerDeath : EntityEventArgs
    {
    }
    public sealed class OnRedFlagCaptured : EntityEventArgs
    {
    }
    public sealed class OnBlueFlagCaptured : EntityEventArgs
    {
    }
    public sealed class OnFlagReturned : EntityEventArgs
    {
        public string? FlagFaction;
    }
    public sealed class OnRedFlagReturned : EntityEventArgs
    {
    }
    public sealed class OnBlueFlagReturned : EntityEventArgs
    {
    }
    public sealed class OnCTFRoundEnd : EntityEventArgs
    {
    }
}
