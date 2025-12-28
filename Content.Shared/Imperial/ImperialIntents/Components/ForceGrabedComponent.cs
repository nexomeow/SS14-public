using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Content.Shared.Damage;

namespace Content.Shared.Imperial.Intent.Components;

/// <summary>
/// Этот компонент на сущности не добавлять и с сущностей не удалять!
/// </summary>
[RegisterComponent, Access(typeof(SharedForceGrabedSystem))]
public sealed partial class ForceGrabedComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool Muted = false;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool Strangling = false;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan StranglingCooldown = TimeSpan.Zero;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(1);

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public EntityUid GraberUid;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float StaminaDamage = 15f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float PushChance = 0.08f;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public DamageSpecifier SuffocatingDamage = new()
    {
        DamageDict = new()
        {
            { "Asphyxiation", 4.1 },
        }
    };
}
