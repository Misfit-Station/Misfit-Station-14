using System.Linq;
using Content.Shared._Misfit.Species.Components;
using Content.Shared.Body;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Misfit.Species.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class EtherealColorSystem : EntitySystem
{
    [Dependency] private readonly SharedPointLightSystem _pointLight = default!;
    [Dependency] private readonly SharedVisualBodySystem _visualBody = default!;

    private static readonly FixedPoint2 TotalHealth = 200;
    private static readonly ProtoId<OrganCategoryPrototype> Head = "Head";

    private static readonly HumanoidVisualLayers[] LayersToForce =
        [HumanoidVisualLayers.Hair, HumanoidVisualLayers.FacialHair];

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EtherealColorComponent, StartingGearEquippedEvent>(OnComponentStartup);
        SubscribeLocalEvent<EtherealColorComponent, DamageChangedEvent>(OnDamageChanged);
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
    }

    private void OnDamageChanged(EntityUid uid, EtherealColorComponent component, DamageChangedEvent args)
    {
        var scalar = Math.Clamp(((TotalHealth - args.Damageable.TotalDamage) / TotalHealth).Float(), 0, 1);
        Color newColor = new(Color.White.RGBA - (Color.White.RGBA - component.InitialColor.RGBA) * scalar);

        if (!_visualBody.TryGatherMarkingsData(uid, null, out var profiles, out _, out var markings))
            return;

        var etherealProfiles = profiles.ToDictionary(pair => pair.Key,
            pair => pair.Value with { SkinColor = newColor });
        _visualBody.ApplyProfiles(uid, etherealProfiles);
        _pointLight.SetColor(uid, newColor);

        if (!markings.TryGetValue(Head, out var headMarkings))
            return;

        foreach (var layer in LayersToForce)
        {
            if (!headMarkings.TryGetValue(layer, out var hairMarkings))
                continue;

            foreach (var marking in hairMarkings)
                marking.SetColor(newColor);
        }

        _visualBody.ApplyMarkings(uid, markings);
    }
}
