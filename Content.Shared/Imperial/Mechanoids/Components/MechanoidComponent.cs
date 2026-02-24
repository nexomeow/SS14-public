using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Color = Robust.Shared.Maths.Color;
using Content.Shared.Imperial.Mechanoids.Prototypes;
using Content.Shared.Imperial.Mechanoids.Systems;
namespace Content.Shared.Imperial.Mechanoids.Components;
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MechanoidComponent : Component
{
    /// <summary>
    /// Модель механоида.
    /// </summary>
    [DataField("modelProto")]
    public ProtoId<MechanoidModelPrototype> Model = "Unknown";

    /// <summary>
    /// Есть ли у механоида шеврон. Шевроны используются для опознания отряда, в котором учавствует механоид. У гражданских и скитальцев нет шевронов.
    /// </summary>
    [DataField("hasChevron"), AutoNetworkedField]
    public bool HasChevron;

    /// <summary>
    /// Цвет шеврона, (если он есть).
    /// </summary>
    [DataField("chevronColor"), AutoNetworkedField]
    public Color ChevronColor = Color.FromHex("#ffffff");
}
