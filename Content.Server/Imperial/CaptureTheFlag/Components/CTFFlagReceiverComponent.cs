using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Content.Shared.Tag;

namespace Content.Server.Imperial.CTF;

[RegisterComponent, Access(typeof(CTFFlagReceiverSystem))]
public sealed partial class CTFFlagReceiverComponent : Component
{
    /// <summary>
    /// Команда к которой принадлежит получатель флага
    /// </summary>
    [DataField("faction")]
    public string Faction = "SimpleNeutral";

    /// <summary>
    /// Вражеская команда
    /// </summary>
    [DataField("enemyFaction")]
    public string EnemyFaction = "SimpleHostile";

    /// <summary>
    /// Звук захвата
    /// </summary>
    [DataField("conquestSound")]
    public SoundSpecifier ConSound = new SoundPathSpecifier("/Audio/Misc/notice1.ogg");

    [DataField]
    public ProtoId<TagPrototype> RedTeamTag = "TonkRedTeam";

    [DataField]
    public ProtoId<TagPrototype> BlueTeamTag = "TonkBlueTeam";

    [DataField]
    public EntityUid FlagEnt;
}
