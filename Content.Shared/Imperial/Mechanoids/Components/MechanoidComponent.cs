using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Systems;

namespace Content.Shared.Imperial.Mechanoids.Components;
[RegisterComponent]
public sealed partial class MechanoidComponent : Component
{
    [DataField("modelProto")]
    public ProtoId<MechanoidModelPrototype> Model = "Unknown";
}
