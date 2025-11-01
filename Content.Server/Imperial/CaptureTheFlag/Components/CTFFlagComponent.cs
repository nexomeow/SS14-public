using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Server.Imperial.CTF;

[RegisterComponent, Access(typeof(CTFFlagSystem))]
public sealed partial class CTFFlagComponent : Component
{
    /// <summary>
    /// Фракция к которой принадлежит флаг
    /// </summary>
    [DataField("faction")]
    public string Faction = "SimpleNeutral";

    /// <summary>
    /// Звук возврата
    /// </summary>
    [DataField("returnSound")]
    public SoundSpecifier RetSound = new SoundPathSpecifier("/Audio/Misc/notice1.ogg");

    [DataField]
    public EntityUid User;

    [DataField]
    public bool OwnerIsSameFaction = false;
}
