using Content.Shared.Imperial.League.Enums;
using Content.Server.Imperial.League.Systems;
using Robust.Shared.GameObjects;

namespace Content.Server.Imperial.League.Components;
[RegisterComponent, Access(typeof(ControlPointSystem))]
public sealed partial class LeagueMemberComponent : Component
{
    /// <summary>
    /// Эта херня нужна для сравнения
    /// </summary>
    [DataField("team")]
    [ViewVariables(VVAccess.ReadOnly)]
    public string TeamMember = "Neutral";
}
