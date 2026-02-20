using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Client.GameObjects;
using Content.Shared.Humanoid;
using Content.Shared.Imperial.Mechanoids.Other;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Systems;
using Content.Shared.Imperial.Mechanoids.Components;

namespace Content.Client.Imperial.Mechanoids;

public partial class MechanoidSystem : SharedMechanoidSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    public IEnumerable<HumanoidVisualLayers> AllowedLayers()
    {
        return new List<HumanoidVisualLayers>
        {
            HumanoidVisualLayers.Head,
            HumanoidVisualLayers.Chest,
            HumanoidVisualLayers.Eyes,
            HumanoidVisualLayers.RHand,
            HumanoidVisualLayers.LHand,
            HumanoidVisualLayers.RLeg,
            HumanoidVisualLayers.LLeg
        };
    }
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MechanoidComponent, MechanoidStartEvent>(OnMechanoidStartEvent);
    }
    private void OnMechanoidStartEvent(EntityUid uid, MechanoidComponent mech, MechanoidStartEvent args)
    {
        SetMechColor(uid, mech);
    }

    private void SetMechColor(EntityUid uid, MechanoidComponent mech)
    {
        if (!_proto.TryIndex(mech.Model, out var modelProto)) return;
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            foreach (var layer in AllowedLayers())
            {
                _sprite.LayerSetColor((uid, sprite), layer, modelProto.ChassisColor);
            }
        }
    }
    private void SetMechName(EntityUid uid, MechanoidComponent mech)
    {
        if (TryComp<MetaDataComponent>(uid, out var metaData))
        {
            if (!_proto.TryIndex(mech.Model, out var modelProto)) return;
            _metaData.SetEntityName(uid, $"{metaData.EntityName} ({modelProto.ModelName}|{modelProto.SubModelName})");
        }
    }
}
