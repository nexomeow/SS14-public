using Robust.Shared.Prototypes;
using Color = Robust.Shared.Maths.Color;
using Content.Shared.Imperial.Mechanoids.Other;

namespace Content.Shared.Imperial.Mechanoids.Prototypes;
[Serializable, Prototype("mechanoidModel")]
public sealed class MechanoidModelPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("modelName")]
    public string ModelName { get; set; } = "UNKW";

    [DataField("subModelName")]
    public string SubModelName { get; set; } = "-";

    [DataField("color")]
    public Color ChassisColor { get; set; } = Color.FromHex("#fffdcb");

    [DataField("faction")]
    public MechanoidFactionEnum Faction { get; set; } = MechanoidFactionEnum.None;

    [DataField("hasAdds")]
    public bool HasAdds { get; set; } = false;

    [DataField("builtInActions")]
    public List<EntProtoId> BuiltInActions = new();
}
