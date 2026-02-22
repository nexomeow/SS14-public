using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Content.Shared.Mind.Components;
using Content.Shared.Imperial.Mechanoids.Other;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Systems;
using Content.Shared.Imperial.Mechanoids.Components;

namespace Content.Client.Imperial.Mechanoids;

public partial class MechanoidSystem : SharedMechanoidSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly ILightManager _light = default!;
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    private MechVisionOverlay? _overlay;
    public IEnumerable<MechanoidVisualLayers> AllowedLayers() // ыыыы щиткод ыыыы
    {
        return new List<MechanoidVisualLayers>
        {
            MechanoidVisualLayers.Head,
            MechanoidVisualLayers.Chest,
            MechanoidVisualLayers.Eyes,
            MechanoidVisualLayers.RHand,
            MechanoidVisualLayers.LHand,
            MechanoidVisualLayers.RLeg,
            MechanoidVisualLayers.LLeg,
            MechanoidVisualLayers.AddsLayer
        };
    }
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MechanoidComponent, MechanoidStartEvent>(OnMechanoidStartEvent);
        SubscribeLocalEvent<MechanoidComponent, MechTurnNightVisionEvent>(OnTurnNightVisionEvent);
        SubscribeLocalEvent<MechanoidComponent, ComponentShutdown>(OnComponentShutdown);
    }
    private void OnMechanoidStartEvent(EntityUid uid, MechanoidComponent mech, MechanoidStartEvent args)
    {
        SetMechColor(uid, mech);
    }
    private void OnComponentShutdown(EntityUid uid, MechanoidComponent mech, ComponentShutdown args)
      =>  DeleteMechVision(uid, mech);
    private void OnMechanoidMindRemoved(EntityUid uid, MechanoidComponent mech, MindRemovedMessage args)
      =>  DeleteMechVision(uid, mech);
    private void DeleteMechVision(EntityUid uid, MechanoidComponent mech)
    {
        if (_overlay != null)
        {
            _overlayManager.RemoveOverlay(_overlay);
            _overlay = null;
        }
        if (!_light.Enabled) _light.Enabled = true;
    }
    private void SetMechColor(EntityUid uid, MechanoidComponent mech)
    {
        if (!_proto.TryIndex(mech.Model, out var modelProto)) return;
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            _sprite.LayerSetColor((uid, sprite), MechanoidVisualLayers.ChevronLayer, mech.ChevronColor);
            foreach (var layer in AllowedLayers())
            {
                _sprite.LayerSetColor((uid, sprite), layer, modelProto.ChassisColor);
            }
        }
    }
    private void OnTurnNightVisionEvent(EntityUid uid, MechanoidComponent mech, ref MechTurnNightVisionEvent args)
    {
        _light.Enabled = !_light.Enabled;

        if (_overlay == null)
        {
            _overlay = new MechVisionOverlay();
            _overlayManager.AddOverlay(_overlay);
        }
        else
        {
            _overlayManager.RemoveOverlay(_overlay);
            _overlay = null;
        }
    }
}
