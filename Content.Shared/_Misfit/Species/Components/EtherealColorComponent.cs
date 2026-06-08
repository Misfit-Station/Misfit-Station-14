using Robust.Shared.GameStates;

namespace Content.Shared._Misfit.Species.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EtherealColorComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color InitialColor;
}
