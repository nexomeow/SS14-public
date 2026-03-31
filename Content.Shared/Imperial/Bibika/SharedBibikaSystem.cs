using Content.Shared.Body.Systems;
using Content.Shared.Body;
using Content.Shared.Gibbing;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Audio;
using Content.Shared.Polymorph;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Imperial.Bibika;

public abstract class SharedBibikaSystem : EntitySystem
{
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSound = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BibikaComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<BibikaComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<BibikaComponent, GibbedBeforeDeletionEvent>(OnGibbedBeforeDeletion);
        SubscribeLocalEvent<BibikaComponent, PolymorphedEvent>(OnPolymorphedBack);
        SubscribeLocalEvent<BibikaComponent, StartCollideEvent>(OnCollide);
    }

    private void OnStartup(EntityUid uid, BibikaComponent bibika, ComponentStartup args)
    {
        bibika.Inside = _container.EnsureContainer<Container>(uid, bibika.InsideContainerId);
    }

    private void OnCollide(EntityUid uid, BibikaComponent bibika, StartCollideEvent args)
    {
        if (!HasComp<BodyComponent>(args.OtherEntity)) return;

        _container.Insert(args.OtherEntity, bibika.Inside);
        _popup.PopupEntity(Loc.GetString("bibika-got-clowned"), uid, uid);
    }

    private void OnShutdown(EntityUid uid, BibikaComponent bibika, ComponentShutdown args)
    {
        FreeVictims(uid, bibika);
    }

    private void OnGibbedBeforeDeletion(EntityUid uid, BibikaComponent bibika, GibbedBeforeDeletionEvent args)
    {
        FreeVictims(uid, bibika);
    }

    private void OnPolymorphedBack(EntityUid uid, BibikaComponent bibika, PolymorphedEvent args)
    {
        FreeVictims(uid, bibika);
    }

    private void FreeVictims(EntityUid uid, BibikaComponent bibika)
    {
        _container.EmptyContainer(bibika.Inside, true, destination: Transform(uid).Coordinates, false);
        _popup.PopupEntity(Loc.GetString("bibika-get-free"), uid, uid);
    }
}
