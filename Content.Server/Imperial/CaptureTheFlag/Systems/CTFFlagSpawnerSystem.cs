using System.Linq;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Map;
using Content.Shared.Hands;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Server.Imperial.CTF.Events;
using Content.Shared.Imperial.CTF;
using Content.Server.Imperial.Components;

namespace Content.Server.Imperial.CTF;

public sealed class CTFFlagSpawnerSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CTFFlagSpawnerComponent, ComponentStartup>(SpawnFirstFlag);
        SubscribeLocalEvent<OnFlagReturned>(ReturnFlag);
    }
    private void SpawnFirstFlag(EntityUid uid, CTFFlagSpawnerComponent flag, ref ComponentStartup args)
    {
        var spawnCoord = _transformSystem.GetMapCoordinates(uid);
        foreach (var entProtoId in flag.FlagPrototype)
        {
            var data = _prototypeManager.Index(entProtoId);
            var offsetCoords = new MapCoordinates(spawnCoord.Position + data.PlacementOffset, spawnCoord.MapId);

            Spawn(entProtoId, offsetCoords);
        }
    }
    private void ReturnFlag(OnFlagReturned ev)
    {
        var faction = ev.FlagFaction;
        var query = EntityQueryEnumerator<CTFFlagSpawnerComponent>();
        while (query.MoveNext(out var uid, out var spawn))
        {
            if (spawn.Team == faction)
                SpawnNewFlag(uid, spawn);
        }
    }
    private void SpawnNewFlag(EntityUid uid, CTFFlagSpawnerComponent flag)
    {
        var spawnCoord = _transformSystem.GetMapCoordinates(uid);
        foreach (var entProtoId in flag.FlagPrototype)
        {
            var data = _prototypeManager.Index(entProtoId);
            var offsetCoords = new MapCoordinates(spawnCoord.Position + data.PlacementOffset, spawnCoord.MapId);

            Spawn(entProtoId, offsetCoords);
            DeclareFlagSpawn(uid, flag);
        }
    }
    private void DeclareFlagSpawn(EntityUid uid, CTFFlagSpawnerComponent flag)
    {
        var station = _stationSystem.GetOwningStation(uid);
        if (station != null)
        {
            _chatSystem.DispatchStationAnnouncement(
            station.Value,
            Loc.GetString("ctf-flag-respawned"),
            Loc.GetString("pvp-sender"),
            playDefaultSound: false,
            flag.ResSound,
            colorOverride: Color.Violet);
        }
    }
}
