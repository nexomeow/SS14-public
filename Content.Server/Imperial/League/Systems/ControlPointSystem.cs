using Robust.Shared.Audio.Systems;
using Filter = Robust.Shared.Player.Filter;
using Robust.Shared.GameObjects;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.DoAfter;
using Content.Server.Chat.Systems;
using Content.Shared.Interaction;
using Content.Shared.Imperial.League.Enums;
using Content.Server.Imperial.League.Events;
using Content.Server.Imperial.League.Components;

namespace Content.Server.Imperial.League.Systems;
public sealed class ControlPointSystem : EntitySystem
{
    [Dependency] private readonly SharedInteractionSystem _interact = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ControlPointComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ControlPointComponent, PointCaptureDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<ControlPointComponent, ActivateInWorldEvent>(OnActivated);
    }
    private void OnStartup(EntityUid uid, ControlPointComponent point, ComponentStartup args)
    {
        switch (point.ByteState)
        {
            case 1:
                point.State = PointType.None;
                break;

            case 2:
                point.State = PointType.KOTH;
                break;

            case 3:
                point.State = PointType.ControlTerritory;
                break;
        }
        UpdateVisual(uid, point);
    }
    private void OnActivated(EntityUid uid, ControlPointComponent point, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        var user = args.User;

        if (!HasComp<LeagueMemberComponent>(user))
            return;

        var teamSystem = EntityManager.System<LeagueMemberSystem>();
        point.LastTouchedTeam = teamSystem.GetTeam(user);

        if (point.LastTouchedTeam == point.OwningTeam)
        {
            _popup.PopupEntity(Loc.GetString("league-point-already-captured"), args.User, PopupType.Small);
            return;
        }

        if (!_interact.InRangeUnobstructed(user, uid))
        {
            _popup.PopupEntity(Loc.GetString("league-point-too-far"), uid, PopupType.SmallCaution);
            return;
        }

        if (point.WasCapturedPreviously)
        {
            if (!point.Changeable)
            {
                _popup.PopupEntity(Loc.GetString("league-point-cannot-recapture"), uid, PopupType.SmallCaution);
                return;
            }
        }
        StartDoAfter(uid, user, point);
    }
    private void StartDoAfter(EntityUid uid, EntityUid user, ControlPointComponent point)
    {
        var doAfter = new DoAfterArgs(EntityManager, user, point.DoAfterLength, new PointCaptureDoAfterEvent(), eventTarget: uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = true,
            RequireCanInteract = true,
            DistanceThreshold = 1f,
        };

        if (!_doAfter.TryStartDoAfter(doAfter))
            return;

        AnnounceAttemptOfCapture(uid, point);
    }
    private void OnDoAfter(EntityUid uid, ControlPointComponent point, PointCaptureDoAfterEvent args)
    {
        if (args.Cancelled)
            return;
        if (args.Handled)
            return;

        args.Handled = true;
        TryRecapturePoint(uid, point);
    }
    private void TryRecapturePoint(EntityUid uid, ControlPointComponent point)
    {
        switch (point.State)
        {
            case PointType.None:
                // Ничего
                break;

            case PointType.KOTH:
                RaiseLocalEvent(new PointCapturedKOTH()
                {
                    Team = point.OwningTeam,
                });
                break;

            case PointType.ControlTerritory:
                RaiseLocalEvent(new PointCapturedControlTerritory()
                {
                    Team = point.OwningTeam,
                });
                break;
        }
        point.OwningTeam = point.LastTouchedTeam;
        point.WasCapturedPreviously = true;

        AnnounceCapture(uid, point);
        UpdateVisual(uid, point);
    }
    private void UpdateVisual(EntityUid uid, ControlPointComponent point)
    {
        var team = point.OwningTeam;
        switch (team)
        {
            case "":
                break;

            case "Neutral":
                point.ByteVisual = 1;
                break;

            case "Red":
                point.ByteVisual = 2;
                break;

            case "Blue":
                point.ByteVisual = 3;
                break;
        }

        if (TryComp<AppearanceComponent>(uid, out var appearance))
        {
            _appearance.SetData(uid, PointVisual.Visual_Layer, point.ByteVisual, appearance);
        }
    }
    private void AnnounceCapture(EntityUid uid, ControlPointComponent point)
    {
        var team = point.OwningTeam;
        _chat.DispatchGlobalAnnouncement(Loc.GetString("league-point-captured", ("team", team)), Loc.GetString("league-sender"), playSound: false, colorOverride: Color.Orange);
        _audio.PlayGlobal("/Audio/Imperial/League/point_capture.ogg", Filter.Broadcast(), true);
    }

    private void AnnounceAttemptOfCapture(EntityUid uid, ControlPointComponent point)
    {
        _chat.DispatchGlobalAnnouncement(Loc.GetString("league-point-attempt-capture"), Loc.GetString("league-sender"), playSound: false, colorOverride: Color.Orange);
        _audio.PlayGlobal("/Audio/Imperial/League/point_capture.ogg", Filter.Broadcast(), true);
    }
/*
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ControlPointComponent>();
        while (query.MoveNext(out var uid, out var point))
        {
            UpdateVisual(uid, point);
        }
    }
*/
}
