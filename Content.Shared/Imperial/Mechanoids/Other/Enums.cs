using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
namespace Content.Shared.Imperial.Mechanoids.Other
{
    [Serializable, NetSerializable]
    public enum MechanoidVisualLayers : byte
    {
        Chest,
        Head,
        Eyes,
        RHand,
        LHand,
        RLeg,
        LLeg,
        Chevron,
        ChevronLayer,
        Adds,
        AddsLayer,

    }
    [Serializable, NetSerializable]
    public enum MechanoidFactionEnum
    {
        None,
        Strangers,
        Cluster,
        OldMachines
    }
}
