using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Containers;

namespace Content.Shared.Imperial.Bibika;

[RegisterComponent]
public sealed partial class BibikaComponent : Component
{
    [DataField]
    public EntProtoId TurnMusicAction = "ActionTurnBibikaMusic";

    [ViewVariables]
    public EntityUid? TurnMusicActionEnt;

    [ViewVariables]
    public string InsideContainerId = "inside";

    [ViewVariables]
    public Container Inside = default!;

    [ViewVariables]
    public bool RoflMusicActive = true;
}

