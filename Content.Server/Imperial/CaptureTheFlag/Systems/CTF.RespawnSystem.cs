/*
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

public sealed class CTFRespawnSystem : EntitySystem
{
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CTFRespawnComponent, MobStateChangedEvent>(OnDeath);
    }
    private void OnDeath(EntityUid uid, CTFRespawnComponent respawn, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Critical | MobState.Dead)
        {
            GetDelayTime(respawn);
            respawn.NeedsToResp = true;
        }
    }
    private EntityUid? FindNearestRespawnPoint(EntityUid uid)
    {
        var transformCompMob = Transform(uid);
        var mapId = transformCompMob.MapID;
        var pos = _transformSystem.GetMapCoordinates(transformCompMob).Position;

        EntityUid? nearest = null;
        var minDist = float.MaxValue;

        var respPointEnumerator = EntityQueryEnumerator<CTFRespawnPointComponent, TransformComponent>();
        while (respPointEnumerator.MoveNext(out var respPointUid, out _, out var transComp))
        {
            if (transComp.MapID != mapId)
                continue;

            var respPos = _transformSystem.GetMapCoordinates(respPointUid).Position;
            var dist = (respPos - pos).LengthSquared();
            if (dist > minDist)
                continue;

            minDist = dist;
            nearest = respPointUid;
        }
        return nearest;
    }
    private static (float xcords, float ycords, string faction) GetRespawnData(CTFRespawnPointComponent component)
    {
        var faction = component.Faction;
        var xcords = component.XCords;
        var ycords = component.YCords;

        return (xcords, ycords, faction);
    }
    private void GetDelayTime(CTFRespawnComponent respawn)
    {
        respawn.RespawnTime = respawn.RespawnTime + _timing.CurTime;
    }
    private void RespawnEntity(EntityUid uid, CTFRespawnComponent respawn)
    {
        respawn.NeedsToResp = false;
        var nearestUid = FindNearestRespawnPoint(uid);
        if (nearestUid == null || !TryComp(nearestUid, out CTFRespawnPointComponent? nearest))
            return;
        var (xcords, ycords, faction) = GetRespawnData(nearest);
    }
    private void DeclareRespawn(EntityUid uid, CTFRespawnComponent respawn)
    {
        var user = uid;
        var station = _stationSystem.GetOwningStation(uid);
        if (station != null)
        {
            _chatSystem.DispatchStationAnnouncement(
            station.Value,
            Loc.GetString("ctf-respawn", ("user", MetaData(user).EntityName)),
            Loc.GetString("pvp-sender"),
            playDefaultSound: false,
            respawn.RespawnSound,
            colorOverride: Color.Violet);
        }
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<CTFRespawnComponent>();
        while (query.MoveNext(out var uid, out var resp))
        {
            if (resp.NeedsToResp)
            {
                if (_timing.CurTime >= resp.RespawnTime)
                    RespawnEntity(uid, resp);
            }
        }
    }
}
*/
