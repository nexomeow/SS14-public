using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Color = Robust.Shared.Maths.Color;
using Content.Shared.Imperial.Mechanoids.Other;

namespace Content.Shared.Imperial.Mechanoids.Prototypes;
[Serializable, Prototype("mechanoidsSquad")]
public sealed class MechanoidSquadPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name")]
    public LocId SquadName { get; set; } = "-";

    [DataField("desc")]
    public LocId SquadDesc { get; set; } = "-";

    [DataField("squadColor")]
    public Color SquadColor { get; set; } = Color.FromHex("#ffffff");

    [DataField("componentsToAdd")]
    public ComponentRegistry Components { get; set; } = new();

    [DataField("staff", required: true)]
    public List<EntProtoId> Staff { get; set; } = new();
}
