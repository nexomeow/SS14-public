using System.Linq;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Content.Shared.Hands;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Server.Imperial.CTF.Events;
using Content.Shared.Imperial.CTF;
using Content.Server.Imperial.Components;

namespace Content.Server.Imperial.CTF;

public sealed class CTFFlagSystem : EntitySystem
{
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CTFFlagComponent, HandSelectedEvent>(OnHandSelected);
    }
    private void OnHandSelected(EntityUid uid, CTFFlagComponent flag, HandSelectedEvent args)
    {
        flag.User = args.User;
        CheckTargetFaction(args.User, flag);
        if (flag.OwnerIsSameFaction)
        {
            RaiseLocalEvent(new OnFlagReturned { FlagFaction = flag.Faction });
            DeclareFlagReturn(uid, flag);
            Del(uid);
        }
    }
    private void DeclareFlagReturn(EntityUid uid, CTFFlagComponent flag)
    {
        var flagEnt = uid;
        var station = _stationSystem.GetOwningStation(uid);
        if (station != null)
        {
            _chatSystem.DispatchStationAnnouncement(
            station.Value,
            Loc.GetString("ctf-flag-returned", ("flag", MetaData(flagEnt).EntityName), ("user", flag.User)),
            Loc.GetString("pvp-sender"),
            playDefaultSound: false,
            flag.RetSound,
            colorOverride: Color.Violet);
        }
    }
    private bool CheckTargetFaction(EntityUid uid, CTFFlagComponent flag)
    {
        var sysMan = IoCManager.Resolve<IEntityManager>();
        var res = false;
        if (!sysMan.TryGetComponent<NpcFactionMemberComponent>(uid, out var factionComponent))
            res = false;
        if (factionComponent != null)
        {
            foreach (var faction in factionComponent.Factions)
            {
                if (flag.Faction == faction)
                    res = true;
            }
        }
        flag.OwnerIsSameFaction = res;
        return res;
    }
}
