using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Map;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Systems;

namespace Content.Shared.Imperial.Mechanoids.Components;
[RegisterComponent]
public sealed partial class MechSquadComponent : Component
{
    /// <summary>
    /// Отряд.
    /// </summary>
    [DataField("squadProto")]
    public ProtoId<MechanoidSquadPrototype> Squad = "MechSquadDebug";

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityCoordinates? SpawnCoords;
}
