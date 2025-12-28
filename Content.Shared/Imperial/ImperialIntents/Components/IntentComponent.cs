using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Audio;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Content.Shared.Imperial.Intent.Enums;

namespace Content.Shared.Imperial.Intent.Components;

[RegisterComponent, Access(typeof(SharedIntentSystem))]
public sealed partial class IntentComponent : Component
{
    /// <summary>
    /// Количество взаимодействий.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public int InteractionsCount = 0;

    /// <summary>
    /// Текущий интент.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public IntentState State = IntentState.White;

    /// <summary>
    /// Может ли энтити изменить интент.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool CanChange = true;

    /// <summary>
    /// Может ли энтити иcпользовать интент прямо сейчас.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool CanUse = true;

    /// <summary>
    /// Финальный кулдаун
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan FinalTime = TimeSpan.Zero;

    /// <summary>
    /// Кулдаун между использованием
    /// </summary>
    [DataField("cooldown"), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan CooldownTime = TimeSpan.FromSeconds(1.5);

    /// <summary>
    /// Имеет ли энтити статус пацифизма?
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool Pacified = false;

    /// <summary>
    /// Коэффициент скорости выполнения действий. По умолчанию он равен стандартным значениям — то есть единице. Чем меньше, тем быстрее.
    /// </summary>
    [DataField("timeCoefficient"), ViewVariables(VVAccess.ReadWrite)]
    public float TimeCoefficient = 1;

    /// <summary>
    /// Коэффициент эффективности выполнения действия. По умолчанию он равен стандартным значениям — то есть единице. Чем больше, тем сильнее будет эффект от действий.
    /// </summary>
    [DataField("effectivenessCoefficient"), ViewVariables(VVAccess.ReadWrite)]
    public float EffectivenessCoefficient = 1;

    #region Зеленый Интент
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier HelpSound = new SoundPathSpecifier("/Audio/Effects/hit_kick.ogg");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float GreenLength = 4f;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public DamageSpecifier DamageRecovery = new()
    {
        DamageDict = new()
        {
            { "Asphyxiation", -7.1 },
        }
    };
    #endregion

    #region Синий Интент
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier DisarmSound = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float StaminaDamage = 15f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float BlueLength = 0f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float PushChance = 0.75f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float DisarmChance = 0.25f;
    #endregion

    #region Желтый Интент
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier GrabSound = new SoundPathSpecifier("/Audio/Effects/stealthoff.ogg");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float YellowLength = 4.5f;
    #endregion

    #region Красный Интент
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float RedLength = 3.5f;
    #endregion

    /// <summary>
    /// В этом параметре определяется длина DoAfterEvent в зависимости от других параметров.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float FinalLength;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float WhiteLength = 0.1f;

    /// <summary>
    /// Последний пользователь, с которым было произведено взаимодействие
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public EntityUid LastTarget;
}
