using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Server.Antag;
using Content.Shared.Destructible;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Components;
using Content.Server.Imperial.BriefingOnMinded.Components;

namespace Content.Server.Imperial.BriefingOnMinded.Systems;

public sealed partial class BriefingOnMindedSystem : EntitySystem
{
    [Dependency] private readonly AntagSelectionSystem _антажке = default!;
    [Dependency] private readonly SharedRoleSystem _ролька = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BriefingOnMindedComponent, MindAddedMessage>(ПриЗаходеГеймераНаРольку);
        SubscribeLocalEvent<BriefingOnMindedComponent, MindRemovedMessage>(ПриВылетеЧеликаСРольки);
    }
    private void ПриЗаходеГеймераНаРольку(EntityUid вместилище, BriefingOnMindedComponent бриф, MindAddedMessage оргументы)
    {
        if (!TryComp<ActorComponent>(вместилище, out var актерКомп))
            return;

        var прататип = бриф.Briefing;
        var ыыыы = оргументы.Mind;
        РассказатьШпендикуЧеЕмуДелать(вместилище, ыыыы, актерКомп.PlayerSession, прататип);
    }
    private void РассказатьШпендикуЧеЕмуДелать(EntityUid вместилище, EntityUid сазнание, ICommonSession шпендик, BriefingOnMindedPrototype радастьГМа)
    {
        _ролька.MindAddRole(сазнание, радастьГМа.рольСазнания, silent: true);

        _антажке.SendBriefing(actorComp.PlayerSession,
            Loc.GetString(радастьГМа.букавы),
            радастьГМа.цвит,
            радастьГМа.путь);
    }
    private void ПриВылетеЧеликаСРольки(EntityUid вместилище, BriefingOnMindedComponent бриф, MindRemovedMessage оргументы)
    {
        //_ролька.MindRemoveRole(args.Mind.Owner, comp.MindRole);
    }
}
