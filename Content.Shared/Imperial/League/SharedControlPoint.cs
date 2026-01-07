using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Shared.Imperial.League.Enums;

[Serializable, NetSerializable]
public enum TeamState : byte
{
    Red,
    Neutral,
    Blue
}

[Serializable, NetSerializable]
public enum PointType : byte
{
    None = 1,
    KOTH = 2,
    ControlTerritory = 3
}

[Serializable, NetSerializable]
public enum PointVisual : byte
{
    Visual_Layer
}

[Serializable, NetSerializable]
public sealed partial class PointCaptureDoAfterEvent : SimpleDoAfterEvent
{}
