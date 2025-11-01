using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.RoundEnd;
using Content.Shared.GameTicking.Components;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Tag;
using Robust.Shared.Utility;
using System.Linq;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Server.Imperial.CTF.Events;
using Content.Shared.Imperial.CTF;
using Content.Server.Imperial.Components;

namespace Content.Server.Imperial.CTF;

public sealed class CTFRuleSystem : GameRuleSystem<CTFRuleComponent>
{
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OnRedFlagCaptured>(RedFlagCaptured);
        SubscribeLocalEvent<OnBlueFlagCaptured>(BlueFlagCaptured);
        SubscribeLocalEvent<OnCTFRoundEnd>(EndRound);
    }

    protected override void Started(EntityUid uid,
        CTFRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
    }
    private void UpdateCond(CTFRuleComponent component)
    {
        if (component.RedFlagCount == component.BlueFlagCount)
        {
            component.RoundEndResult = RoundEndConditions.StaleMate;
        }
        else if (component.RedFlagCount > component.BlueFlagCount)
        {
            component.RoundEndResult = RoundEndConditions.RedTeamWin;
            component.WinType = RoundEndWinType.SmallRedTeamWin;
        }
        else if (component.RedFlagCount < component.BlueFlagCount)
        {
            component.RoundEndResult = RoundEndConditions.BlueTeamWin;
            component.WinType = RoundEndWinType.SmallBlueTeamWin;
        }
        if (component.RedFlagCount >= component.FlagCountGoal
        | component.BlueFlagCount >= component.FlagCountGoal)
        {
            if (component.RedFlagCount > component.BlueFlagCount)
                component.WinType = RoundEndWinType.BigRedTeamWin;
            if (component.RedFlagCount < component.BlueFlagCount)
                component.WinType = RoundEndWinType.BigBlueTeamWin;
            var ev = new OnCTFRoundEnd();
            RaiseLocalEvent(ev);
        }
    }
    private void BlueFlagCaptured(OnBlueFlagCaptured ev)
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out var ctf, out _))
        {
            ctf.BlueFlagCount = ctf.BlueFlagCount + 1;
            UpdateCond(ctf);
        }
    }
    private void RedFlagCaptured(OnRedFlagCaptured ev)
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out var ctf, out _))
        {
            ctf.RedFlagCount = ctf.RedFlagCount + 1;
            UpdateCond(ctf);
        }
    }
    private void EndRound(OnCTFRoundEnd ev)
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out var ctf, out _))
        {
            if (GameTicker.IsGameRuleActive("TonkCTFRule"))
            {
                _roundEndSystem.EndRound();
                GameTicker.EndGameRule(uid);
            }
        }
    }
    protected override void AppendRoundEndText(EntityUid uid,
        CTFRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        var ctf = Loc.GetString("ctf-main");
        args.AddLine(ctf);
        var winText = Loc.GetString($"ctf-wintype-{component.WinType.ToString().ToLower()}");
        args.AddLine(winText);
        var resText = Loc.GetString($"ctf-{component.RoundEndResult.ToString().ToLower()}");
        args.AddLine(resText);
    }
}
