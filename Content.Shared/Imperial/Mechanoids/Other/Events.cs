using Robust.Shared.GameObjects;
using Content.Shared.Actions;
namespace Content.Shared.Imperial.Mechanoids.Other
{
    [ByRefEvent]
    public struct MechanoidStartEvent(EntityUid uid)
    {
        public readonly EntityUid Uid = uid;
    }

    [ByRefEvent]
    public sealed partial class MechTurnNightVisionEvent : InstantActionEvent;
}
