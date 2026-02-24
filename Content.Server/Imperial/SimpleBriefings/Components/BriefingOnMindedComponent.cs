using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Roles.Components;
using Content.Server.Imperial.BriefingOnMinded.Systems;

namespace Content.Server.Imperial.BriefingOnMinded.Components;

/// <summary>
/// Брифинг, который будет проведен игроку при вселении в эту сущность
/// </summary>
[RegisterComponent]
public sealed partial class BriefingOnMindedComponent : Component
{
    /// <summary>
    /// Прототип брифинга
    /// </summary>
    [DataField("briefProto", required: true)]
    public string Briefing = string.Empty;
}
