using Robust.Shared.Serialization;
using Content.Shared.DoAfter;
using Content.Shared.Alert;

namespace Content.Shared.Imperial.Intent.Events;

[Serializable, NetSerializable]
public sealed partial class IntentDoAfterEvent : SimpleDoAfterEvent
{}

/// <summary>
/// Ивент райсится при нажатии на алерты удушения и захвата
/// </summary>
public sealed partial class ForceGrabedAlertEvent : BaseAlertEvent;

// Это ивенты, которые производятся на сущность при использовании на ней интентов
// Если вам нужно, чтобы после применения определенного интента на сущность
// с вашей системой что-то происходило — используйте ниже представленные
// Они используются в том случае, если цель находится в захвате
// Кроме синего и белого. Синий используется всегда, а белый? Он тут по приколу лол

[ByRefEvent]
public struct WhiteIntentedEvent(EntityUid uid, EntityUid target)
{
    public readonly EntityUid Uid = uid;
    public readonly EntityUid? Target = target;
}

[ByRefEvent]
public struct GreenIntentedEvent(EntityUid uid, EntityUid target)
{
    public readonly EntityUid Uid = uid;
    public readonly EntityUid? Target = target;
}

[ByRefEvent]
public struct BlueIntentedEvent(EntityUid uid, EntityUid target)
{
    public readonly EntityUid Uid = uid;
    public readonly EntityUid? Target = target;
}

[ByRefEvent]
public struct YellowIntentedEvent(EntityUid uid, EntityUid target)
{
    public readonly EntityUid Uid = uid;
    public readonly EntityUid? Target = target;
}

[ByRefEvent]
public struct RedIntentedEvent(EntityUid uid, EntityUid target)
{
    public readonly EntityUid Uid = uid;
    public readonly EntityUid? Target = target;
}
