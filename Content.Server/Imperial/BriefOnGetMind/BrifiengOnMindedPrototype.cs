using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Content.Shared.Roles.Components;

namespace Content.Server.Imperial.BriefingOnMinded;
[Serializable, Prototype("brifieng")]
public sealed class BriefingOnMindedPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("briefText", required: true)]
    public LocId букавы { get; set; }

    [DataField("textColor", required: true)]
    public string цвит { get; set; } = string.Empty;

    [DataField("briefSound", required: true)]
    public SoundPathSpecifier путь = default!;

    [DataField("mindRole", required: true)]
    public EntProtoId<MindRoleComponent> рольСазнания { get; set; } = string.Empty;
}
