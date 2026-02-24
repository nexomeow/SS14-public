using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Actions;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Imperial.Mechanoids.Other;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Components;

namespace Content.Shared.Imperial.Mechanoids.Systems;

public abstract class SharedMechanoidSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MechanoidComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<MechanoidComponent, ExaminedEvent>(OnExamine);
    }
    // Локальные подписки
    private void OnInit(EntityUid uid, MechanoidComponent mech, ComponentInit args)
    {
        var ev = new MechanoidStartEvent();
        RaiseLocalEvent(uid, ref ev);

        GrantActions(uid, mech);
        SetSpriteLayers(uid, mech);
        SetMechName(uid, mech);
    }
    private void OnExamine(EntityUid uid, MechanoidComponent mech, ExaminedEvent args)
    {
        if (!_proto.TryIndex(mech.Model, out var modelProto)) return;

        args.PushMarkup(Loc.GetString("mechanoid-depend-on-this-faction-examine", ("depended", modelProto.Faction.ToString())));
        args.PushMarkup(Loc.GetString("mechanoid-model-examine", ("model", modelProto.ModelName), ("subModel", modelProto.SubModelName)));
    }
    // Методы
    public void SetSpriteLayers(EntityUid uid, MechanoidComponent? mech = null)
    {
        if (!Resolve(uid, ref mech))
            return;

        if (!_proto.TryIndex(mech.Model, out var modelProto)) return;

        if (TryComp<AppearanceComponent>(uid, out var appearance))
        {
            _appearance.SetData(uid, MechanoidVisualLayers.Adds, modelProto.HasAdds, appearance);
            _appearance.SetData(uid, MechanoidVisualLayers.Chevron, mech.HasChevron, appearance);
        }
    }
    private void GrantActions(EntityUid uid, MechanoidComponent mech)
    {
        if (!_proto.TryIndex(mech.Model, out var modelProto)) return;

        if (!modelProto.BuiltInActions.Any()) return;

        foreach (var action in modelProto.BuiltInActions)
        {
            EntityUid? actionEnt = null;
            _actions.AddAction(uid, ref actionEnt, action);
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
