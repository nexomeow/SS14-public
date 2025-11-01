using Content.Server.RoundEnd;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Server.Imperial.CTF.Events;
using Content.Server.Imperial.CTF;
using Content.Shared.Imperial.CTF;

namespace Content.Server.Imperial.Components;

[RegisterComponent, Access(typeof(CTFRuleSystem))]
public sealed partial class CTFRuleComponent : Component
{
    [DataField]
    public RoundEndConditions RoundEndResult = RoundEndConditions.StaleMate;

    [DataField]
    public RoundEndWinType WinType = RoundEndWinType.StaleMate;

    [DataField("flagsToFinishRound")]
    public int FlagCountGoal = 3;

    [DataField]
    public int RedFlagCount = 0;

    [DataField]
    public int BlueFlagCount = 0;

    [DataField]
    public TimeSpan RoundTime = TimeSpan.FromSeconds(1200);
}
