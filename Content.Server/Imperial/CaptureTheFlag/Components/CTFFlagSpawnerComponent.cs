using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Imperial.CTF;

[RegisterComponent, Access(typeof(CTFFlagSpawnerSystem))]
public sealed partial class CTFFlagSpawnerComponent : Component
{
    /// <summary>
    /// Команда к которой принадлежит спавнер флага
    /// </summary>
    [DataField("team")]
    public string Team = "Neutral";

    /// <summary>
    /// Прототип флага
    /// </summary>
    [DataField("flagProto", required: true)]
    public List<EntProtoId> FlagPrototype;

    /// <summary>
    /// Звук спавна
    /// </summary>
    [DataField("spawnSound")]
    public SoundSpecifier ResSound = new SoundPathSpecifier("/Audio/Misc/notice1.ogg");
}
