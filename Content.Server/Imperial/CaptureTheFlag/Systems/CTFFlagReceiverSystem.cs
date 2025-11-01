using System.Linq;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Content.Shared.Tag;
using Content.Shared.Hands;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Server.Imperial.CTF.Events;
using Content.Shared.Imperial.CTF;
using Content.Server.Imperial.Components;

namespace Content.Server.Imperial.CTF;

public sealed class CTFFlagReceiverSystem : EntitySystem
{
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CTFFlagReceiverComponent, StartCollideEvent>(OnFlagCollide);
    }
    private void OnFlagCollide(EntityUid uid, CTFFlagReceiverComponent flag, ref StartCollideEvent args)
    {
        flag.FlagEnt = args.OtherEntity;
        if (!HasComp<CTFFlagComponent>(flag.FlagEnt))
            return;

        if (_tag.HasTag(flag.FlagEnt, flag.RedTeamTag) && flag.Faction == "TonkTeamBlue")
        {
            var goal = flag.FlagEnt;
            RaiseLocalEvent(new OnBlueFlagCaptured());
            RaiseLocalEvent(new OnFlagReturned { FlagFaction = flag.EnemyFaction });
            DeclareFlagCon(uid, flag);
            Del(goal);
        }
        if (_tag.HasTag(flag.FlagEnt, flag.BlueTeamTag) && flag.Faction == "TonkTeamRed")
        {
            var goal = flag.FlagEnt;
            RaiseLocalEvent(new OnRedFlagCaptured());
            RaiseLocalEvent(new OnFlagReturned { FlagFaction = flag.EnemyFaction });
            DeclareFlagCon(uid, flag);
            Del(goal);
        }
    }
    private void DeclareFlagCon(EntityUid uid, CTFFlagReceiverComponent flag)
    {
        var station = _stationSystem.GetOwningStation(uid);
        if (station != null)
        {
            _chatSystem.DispatchStationAnnouncement(
            station.Value,
            Loc.GetString("ctf-flag-con", ("flag", MetaData(flag.FlagEnt).EntityName)),
            Loc.GetString("pvp-sender"),
            playDefaultSound: false,
            flag.ConSound,
            colorOverride: Color.Violet);
        }
    }
}
