
using Content.Shared._Misfit.Species.Components;
using Content.Shared._Misfit.Species.Systems;
using Content.Shared.Body;
using Content.Shared.Roles;
using Robust.Shared.Utility;

namespace Content.Server._Misfit.Species.Systems;
public sealed partial class EtherealColorSystem : SharedEtherealColorSystem
{
    [Dependency] private SharedPointLightSystem _pointLight = default!;
    [Dependency] private SharedVisualBodySystem _visualBody = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EtherealColorComponent, StartingGearEquippedEvent>(OnComponentStartup);
    }

    private void OnComponentStartup(EntityUid uid, EtherealColorComponent component, ref StartingGearEquippedEvent args)
    {
        if (!_pointLight.TryGetLight(uid, out var lightComp))
            return;

        if (!_visualBody.TryGatherMarkingsData(uid, null, out var profiles, out _, out var _))
            return;

        if (!profiles.TryFirstOrNull(out var profile))
            return;

        var skinColor = profile.Value.Value.SkinColor; // Every "organ" should have the same skin color

        _pointLight.SetColor(uid, skinColor, lightComp);

        component.InitialColor = skinColor;

        Dirty(uid, component);
    }
}
