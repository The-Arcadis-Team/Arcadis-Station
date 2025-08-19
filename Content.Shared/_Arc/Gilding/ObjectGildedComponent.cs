using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Arc.Admemery;


/// <summary>
/// "I do believe I am gold."
/// </summary>
[RegisterComponent, NetworkedComponent]
//[Access(typeof(ComputerDiskSystem))]
public sealed partial class ObjectGildedComponent : Component
{
    /// <summary>
    /// How long until the gilded thing can escape their golden hell
    /// </summary>
    [DataField]
    public TimeSpan GildedDuration = TimeSpan.FromMinutes(1);

    /// <summary>
    ///    The sound that plays when an object is gilded
    /// </summary>
    // Thank you for thinking of this, Copilot

    [DataField]
    public SoundSpecifier? GildedSound = new SoundPathSpecifier("/Audio/_ARC/gilded.ogg");

}

[Serializable, NetSerializable]

public enum GildedVisuals : byte
{
    Gilded,
}

