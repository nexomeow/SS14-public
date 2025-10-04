using Robust.Shared.Console;
using Content.Shared.Administration;
using Content.Server.Administration;
using Robust.Shared.Prototypes;
using System.Linq;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Audio.Systems;
// using Content.Server.Imperial.ErtCall;
using Content.Server.Chat.Systems;


namespace Content.Server.Imperial.TemplateAnnounce;

[AdminCommand(AdminFlags.Admin)]
public sealed class TemplateAnnounceCommand : LocalizedCommands
{
    public string Description => Loc.GetString("templateannounce-desc");
    public string Help => Loc.GetString("templateannounce-help");
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override string Command => "send_templatedannounce";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = _prototype
                .EnumeratePrototypes<TemplateAnnouncePrototype>()
                .Select(p => new CompletionOption(p.ID, p.Desc));

            return CompletionResult.FromHintOptions(options.OrderBy(x => x.Value, StringComparer.Ordinal).ToArray(), Loc.GetString("templateannounce-prototype"));
        }

        return CompletionResult.Empty;
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var station = _stationSystem.GetOwningStation(uid, transComp);
        var chatSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>();
        var announcePrototype = args[0];
        var prototypeManager = _prototype;

        if (!prototypeManager.TryIndex<TemplateAnnouncePrototype>(announcePrototype, out var proto))
        {
            shell.WriteError(Loc.GetString("templateannounce-error-proto-doesnt-exist", ("announcePrototype", announcePrototype)));
            return;

            if (station != null)
            {
                _chatSystem.DispatchStationAnnouncement(station.Value, Loc.GetString(proto.MessageText), playDefaultSound: false, colorOverride: proto.AnnounceColor);
                _audio.PlayGlobal(proto.AnnounceSound, Filter.Broadcast(), true);
                shell.WriteLine(Loc.GetString("templateannounce-success"));
                return;
            }
            else
            {
                shell.WriteError(Loc.GetString("templateannounce-error"));
                return;
            }
        }
    }
}

