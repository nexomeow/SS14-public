using System.Linq;
using System.Numerics;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Imperial.Mechanoids.Other;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Components;

namespace Content.Shared.Imperial.Mechanoids.Systems;

public abstract class SharedMechSquadSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedMechanoidSystem _mechnanoid = default!;
    private ISawmill _sawmill = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MechSquadComponent, ComponentInit>(OnInit);
    }
    private void OnInit(EntityUid uid, MechSquadComponent squad, ComponentInit args)
    {
        if (!_proto.TryIndex(squad.Squad, out var squadProto)) return;

        if (!squadProto.Staff.Any())
        {
            _sawmill.Error($"{squadProto.ID} does not have staff members to spawn!");
            return;
        }

        squad.SpawnCoords = Transform(uid).Coordinates;

        foreach (var mechs in squadProto.Staff)
        {
            var spawned = EntityManager.CreateEntityUninitialized(mechs, (EntityCoordinates)squad.SpawnCoords);
            if (TryComp<MechanoidComponent>(spawned, out var mechComp))
            {
                mechComp.HasChevron = true;
                mechComp.ChevronColor = squadProto.SquadColor;
            }
            else
            {
                _sawmill.Error($"{spawned} does not have MechanoidComponent!!");
                return;
            }

            if (!squadProto.Components.Any())
                _sawmill.Debug($"{squadProto.ID} does not have components to add.");
            else
                EntityManager.AddComponents(spawned, squadProto.Components);

            EntityManager.InitializeAndStartEntity(spawned);
            Dirty(spawned, mechComp!);
        }

        Del(uid);
    }
}
