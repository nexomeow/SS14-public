using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Color = Robust.Shared.Maths.Color;
using Content.Server.Antag;
using Content.Shared.Destructible;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Components;
using Content.Server.Imperial.BriefingOnMinded.Components;
using System.Drawing;

namespace Content.Server.Imperial.BriefingOnMinded.Systems;

public sealed partial class BriefingOnMindedSystem : EntitySystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BriefingOnMindedComponent, MindAddedMessage>(OnMindAdded);

    }
    private void OnMindAdded(EntityUid uid, BriefingOnMindedComponent brief, MindAddedMessage args)
    {
        if (!TryComp<ActorComponent>(uid, out var actorComp))
            return;

        if (!_prototype.TryIndex<BriefingOnMindedPrototype>(brief.Briefing, out var proto))
            return;

        ProvideBriefing(uid, args.Mind, actorComp.PlayerSession, proto);
    }
    private void ProvideBriefing(EntityUid uid, EntityUid mind, ICommonSession player, BriefingOnMindedPrototype briefProto)
    {
        _role.MindAddRole(mind, briefProto.MindRole, silent: true);

        var color = Color.FromHex(briefProto.Color);
        _antag.SendBriefing(player,
            Loc.GetString(briefProto.Text),
            color,
            briefProto.Path);
    }
}
