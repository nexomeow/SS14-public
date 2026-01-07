using Robust.Shared.GameObjects;
using Content.Shared.Imperial.League.Enums;
using Content.Server.Imperial.League.Components;

namespace Content.Server.Imperial.League.Systems;
public sealed class LeagueMemberSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LeagueMemberComponent, ComponentStartup>(OnStartup);
    }
    private void OnStartup(EntityUid uid, LeagueMemberComponent point, ComponentStartup args)
    {
    }
    public string GetTeam(EntityUid uid, LeagueMemberComponent? member = null)
    {
        if (!Resolve(uid, ref member))
            return "";

        return member.TeamMember;
    }
}
