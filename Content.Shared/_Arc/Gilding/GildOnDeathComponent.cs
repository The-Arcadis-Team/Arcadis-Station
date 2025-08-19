using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Arc.Admemery;

/// <summary>
/// Component for GildOnDeath system
/// </summary>
[RegisterComponent, NetworkedComponent]
//[Access(typeof(ComputerDiskSystem))]
public sealed partial class WeaponGildOnCritComponent : Component
{
    /// <summary>
    /// How long until the gilded player can escape their golden hell when gilded
    /// </summary>
    [DataField]
    public TimeSpan GildedDuration = TimeSpan.FromMinutes(1);


}
