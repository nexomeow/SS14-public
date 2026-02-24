using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Content.Shared.Roles.Components;

namespace Content.Server.Imperial.BriefingOnMinded;
[Serializable, Prototype("briefing")]
public sealed class BriefingOnMindedPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("briefText", required: true)]
    public LocId Text { get; set; }

    [DataField("textColor", required: true)]
    public string Color { get; set; } = string.Empty;

    [DataField("briefSound", required: true)]
    public SoundPathSpecifier Path = default!;

    [DataField("mindRole", required: true)]
    public EntProtoId<MindRoleComponent> MindRole { get; set; } = string.Empty;
}
