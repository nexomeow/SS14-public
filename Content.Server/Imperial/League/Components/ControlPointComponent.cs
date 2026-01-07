using Robust.Shared.Timing;
using Content.Shared.Imperial.League.Enums;
using Content.Server.Imperial.League.Systems;
using Robust.Shared.GameObjects;

namespace Content.Server.Imperial.League.Components;
[RegisterComponent, Access(typeof(ControlPointSystem))]
public sealed partial class ControlPointComponent : Component
{
    /// <summary>
    /// Эта херня нужна для сравнения
    /// </summary>
    [DataField("team")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string OwningTeam = "Neutral";

    /// <summary>
    /// Эта херня нужна для сравнения
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public string LastTouchedTeam;

    /// <summary>
    /// Время требуемое для захвата
    /// </summary>
    [DataField("captureTime")]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan DoAfterLength = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Тип точки. От этого зависит что она пошлет в геймрул при захвате
    /// Следующие два параметра отвечают за это
    /// </summary>
    [DataField("pointType", required: true)]
    public byte ByteState = 0;

    [ViewVariables(VVAccess.ReadOnly)]
    public PointType State = PointType.None;

    /// <summary>
    /// Можно ли перезахватить?
    /// </summary>
    [DataField("changeable")]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Changeable = true;

    /// <summary>
    /// Проверочка захватывалась ли точка ранее
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool WasCapturedPreviously = false;

    /// <summary>
    /// Определение визуала
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public byte ByteVisual = 1;
}
