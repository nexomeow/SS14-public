using Robust.Shared.GameObjects;

namespace Content.Shared.Imperial.Mechanoids.Other;

[ByRefEvent]
public struct MechanoidStartEvent(EntityUid uid)
{
    public readonly EntityUid Uid = uid;
}
