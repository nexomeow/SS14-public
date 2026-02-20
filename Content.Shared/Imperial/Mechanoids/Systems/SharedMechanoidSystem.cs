using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Humanoid;
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
    }
    private void OnExamine(EntityUid uid, MechanoidComponent mech, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("mechanoid-depend-on-this-fation"));
        args.PushMarkup(Loc.GetString("mechanoid-model"));
    }
    // Методы
}
