using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Server.Imperial.BriefingOnMinded.Systems;

[RegisterComponent]
namespace Content.Server.Imperial.BriefingOnMinded.Components;

public sealed partial class BriefingOnMindedComponent : Component
{
    /// <summary>
    /// Прототип брифинга
    /// </summary>
    [DataField("briefProto", required: true)]
    public ProtoId<BriefingOnMindedPrototyp>? Briefing;
}
