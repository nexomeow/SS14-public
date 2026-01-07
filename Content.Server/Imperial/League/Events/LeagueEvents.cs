using Robust.Shared.GameObjects;

namespace Content.Server.Imperial.League.Events;

public sealed class PointCapturedKOTH : EntityEventArgs
{
    public string? Team;
}
public sealed class PointCapturedControlTerritory : EntityEventArgs
{
    public string? Team;
}
