using Content.Shared._Misfit.Species.Components;
using Content.Shared._Misfit.Species.Systems;
using Robust.Client.GameObjects;

namespace Content.Client._Misfit.Species.Systems;
public sealed partial class EtherealColorSystem : SharedEtherealColorSystem
{
    [Dependency] private SpriteSystem _sprite = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EtherealColorComponent, ComponentStartup>(OnComponentStartup);
    }

    private void OnComponentStartup(EntityUid uid, EtherealColorComponent component, ComponentStartup args)
    {
        if(!HasComp<EtherealCrystalComponent>(uid))
            return;

        _sprite.SetColor(uid, component.InitialColor);
    }
}
