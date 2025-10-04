using Robust.Shared.Prototypes;

namespace Content.Server.Imperial.TemplateAnnounce;

[Serializable, Prototype("templatedGlobalAnnounce")]
public sealed class TemplateAnnouncePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Отправитель
    /// </summary>
    [DataField("sender")]
    public string Sender { get; set; } = string.Empty;

    /// <summary>
    /// Текст сообщения
    /// </summary>
    [DataField("messageText")]
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// Описание для объяснения, что это за сообщение
    /// </summary>
    [DataField("desc")]
    public string AnnounceDesc { get; set; } = string.Empty;

    /// <summary>
    /// Звук при отправке глобального сообщения
    /// </summary>
    [DataField("announceSound")]
    public SoundSpecifier? AnnounceSound = new SoundPathSpecifier("/Audio/Announcements/announce.ogg");

    [DataField("announceColor")]
    public Color AnnounceColor = Color.FromHex("#FFFF00");
}
